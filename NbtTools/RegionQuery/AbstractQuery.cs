using Microsoft.Extensions.Logging;
using NbtTools.Geography;
using NbtTools.Mca;
using NbtTools.Nbt;
using SharpNBT;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VersionedCompoundTag = NbtTools.Versioned<SharpNBT.CompoundTag>;

namespace NbtTools.RegionQuery
{
    public abstract class AbstractQuery
    {
        private readonly NbtReader Reader = new NbtReader();
        private readonly ILogger<AbstractQuery> Logger;

        protected abstract string ElementKey { get; }

        protected AbstractQuery(ILogger<AbstractQuery> logger)
        {
            Logger = logger;
        }

        public QueryResult<VersionedCompoundTag> GetData(Cuboid zone)
        {
            var chunks = zone.GetAllChunks();
            var regions = chunks.Select(c => c.Region).Distinct();

            var result = new QueryResult<VersionedCompoundTag>();

            foreach (var region in regions)
            {
                var regionTags = ReadTagsOfRegion(region, zone);
                result.AddRange(regionTags);
            }

            return result;
        }

        private QueryResult<VersionedCompoundTag> ReadTagsOfRegion(Region region, Cuboid zone)
        {
            var result = new QueryResult<VersionedCompoundTag>();

            var file = GetFile(zone.Dimension, region.GetFileName());
            var regionChunks = zone.GetAllChunks().Where(c => c.Region.Equals(region));

            foreach (Chunk c in regionChunks)
            {
                var chunk = file.GetChunk(c.GetChunkId());
                if (chunk.Length <= 0)
                {
                    continue;
                }

                try
                {
                    var chunkTags = ReadTagsOfChunk(chunk, zone);
                    result.AddRange(chunkTags);
                }
                catch (UnreadableChunkException e)
                {
                    Logger.LogError(e, "Could not read chunk {0} from region file {1}", c, region);
                    result.UnreadableChunks.Add(c);
                }
            }

            return result;
        }

        private QueryResult<VersionedCompoundTag> ReadTagsOfChunk(ChunkEntry chunk, Cuboid zone)
        {
            var result = new QueryResult<VersionedCompoundTag>();

            var chunkMainTag = Reader.ReadChunk(chunk);
            if (IsValidChunk(chunkMainTag))
            {
                var dataVersion = chunkMainTag["DataVersion"] as IntTag;
                var data = chunkMainTag[ElementKey] as ListTag;

                if (data != null)
                {
                    foreach (var entity in data)
                    {
                        var compoundTag = entity as CompoundTag;

                        // Ignore entities that are in the chunk, but outside of the selection
                        // (in chunks containing the selection limits)
                        if (IsInZone(compoundTag, zone))
                        {
                            result.Result.Add(new VersionedCompoundTag(compoundTag, dataVersion));
                        }
                    }
                }
            }

            return result;
        }

        protected abstract McaFile GetFile(string dimension, string fileName);

        protected abstract bool IsInZone(CompoundTag element, Cuboid zone);

        protected abstract bool IsValidChunk(CompoundTag chunkMainTag);
    }
}
