using fNbt;
using Microsoft.Extensions.Logging;
using NbtTools.Geography;
using NbtTools.Mca;
using NbtTools.Nbt;
using System.Linq;
using VersionedNbtCompound = NbtTools.Versioned<fNbt.NbtCompound>;

namespace NbtTools.RegionQuery
{
    public abstract class AbstractQuery
    {
        private readonly ChunkNbtReader Reader = new ChunkNbtReader();
        private readonly ILogger<AbstractQuery> Logger;

        protected abstract string ElementKey { get; }

        protected AbstractQuery(ILogger<AbstractQuery> logger)
        {
            Logger = logger;
        }

        public QueryResult<VersionedNbtCompound> GetData(Cuboid zone)
        {
            var chunks = zone.GetAllChunks();
            var regions = chunks.Select(c => c.Region).Distinct();

            var result = new QueryResult<VersionedNbtCompound>();

            foreach (var region in regions)
            {
                var regionTags = ReadTagsOfRegion(region, zone);
                result.AddRange(regionTags);
            }

            return result;
        }

        private QueryResult<VersionedNbtCompound> ReadTagsOfRegion(Region region, Cuboid zone)
        {
            var result = new QueryResult<VersionedNbtCompound>();

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

        private QueryResult<VersionedNbtCompound> ReadTagsOfChunk(ChunkEntry chunk, Cuboid zone)
        {
            var result = new QueryResult<VersionedNbtCompound>();

            var chunkMainTag = Reader.ReadChunk(chunk);
            if (IsValidChunk(chunkMainTag))
            {
                var dataVersionTag = chunkMainTag["DataVersion"] as NbtInt;
                var data = chunkMainTag[ElementKey] as NbtList;

                if (data != null)
                {
                    foreach (var entity in data)
                    {
                        var NbtCompound = entity as NbtCompound;

                        // Ignore entities that are in the chunk, but outside of the selection
                        // (in chunks containing the selection limits)
                        if (IsInZone(NbtCompound, zone))
                        {
                            result.Result.Add(new VersionedNbtCompound(NbtCompound, dataVersionTag.Value));
                        }
                    }
                }
            }

            return result;
        }

        protected abstract McaFile GetFile(string dimension, string fileName);

        protected abstract bool IsInZone(NbtCompound element, Cuboid zone);

        protected abstract bool IsValidChunk(NbtCompound chunkMainTag);
    }
}
