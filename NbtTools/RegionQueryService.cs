using Microsoft.Extensions.Logging;
using NbtTools.Geography;
using NbtTools.Mca;
using NbtTools.Nbt;
using SharpNBT;
using System.Collections.Generic;
using System.Linq;

using CompoundTags = System.Collections.Generic.ICollection<NbtTools.Versioned<SharpNBT.CompoundTag>>;

namespace NbtTools
{
    public class RegionQueryService
    {
        private readonly NbtReader Reader = new NbtReader();
        private readonly McaFileFactory McaFileFactory;
        private readonly ILogger<RegionQueryService> Logger;

        public RegionQueryService(McaFileFactory mcaFileFactory, ILogger<RegionQueryService> logger)
        {
            McaFileFactory = mcaFileFactory;
            Logger = logger;
        }

        public QueryResult<CompoundTags> GetEntitiesDataSource(Cuboid zone)
        {
            var chunks = zone.GetAllChunks();
            var regions = chunks.Select(c => c.Region).Distinct();

            var chunksEntities = new List<Versioned<CompoundTag>>();
            var missingChunks = new List<Chunk>();

            foreach (var region in regions)
            {
                var file = McaFileFactory.GetEntitiesFile(zone.Dimension, region.GetFileName());
                var regionChunks = chunks.Where(c => c.Region.Equals(region));

                foreach (Chunk c in regionChunks)
                {
                    var chunk = file.GetChunk(c.GetChunkId());
                    if (chunk.Length > 0)
                    {
                        try
                        {
                            var chunkMainTag = Reader.ReadChunk(chunk);
                            var dataVersion = chunkMainTag["DataVersion"] as IntTag;
                            var chunkEntitiesCollection = chunkMainTag["Entities"] as ListTag;
                            if (chunkEntitiesCollection != null)
                            {
                                foreach (var entity in chunkEntitiesCollection)
                                {
                                    chunksEntities.Add(new Versioned<CompoundTag>((CompoundTag)entity, dataVersion));
                                }
                            }
                        }
                        catch (UnreadableChunkException e) 
                        {
                            Logger.LogError(e, "Could not read chunk {0} from entity file {1}", c, region);
                            missingChunks.Add(c);
                        }
                    }
                }
            }

            return new QueryResult<CompoundTags>(chunksEntities, missingChunks);
        }
        
        public QueryResult<CompoundTags> GetBlockEntitiesDataSource(Cuboid zone)
        {
            var chunks = zone.GetAllChunks();
            var regions = chunks.Select(c => c.Region).Distinct();

            var data = new List<Versioned<CompoundTag>>();
            var missingChunks = new List<Chunk>();

            foreach (var region in regions)
            {
                var file = McaFileFactory.GetRegionFile(zone.Dimension, region.GetFileName());
                var headers = file.GetHeader();
                var regionChunks = chunks.Where(c => c.Region.Equals(region)).ToList();

                foreach (Chunk c in regionChunks)
                {
                    var chunk = file.GetChunk(c.GetChunkId());
                    if (chunk.Length > 0)
                    {
                        try
                        {
                            var chunkMainTag = Reader.ReadChunk(chunk);
                            var status = chunkMainTag["Status"] as StringTag;
                            if (status != null && status == "minecraft:full")
                            {
                                var dataVersion = chunkMainTag["DataVersion"] as IntTag;
                                var blockEntities = chunkMainTag["block_entities"] as ListTag;
                                foreach (var element in blockEntities)
                                {
                                    var blockEntity = element as CompoundTag;

                                    // Ignore entities that are in the chunk, but outside of the selection
                                    // (in chunks containing the selection limits)
                                    Point position = new Point(
                                        (blockEntity["x"] as IntTag).Value,
                                        (blockEntity["y"] as IntTag).Value,
                                        (blockEntity["z"] as IntTag).Value
                                    );

                                    if (zone.Contains(position))
                                    {
                                        data.Add(new Versioned<CompoundTag>(blockEntity, dataVersion));
                                    }
                                }
                            }
                        }
                        catch (UnreadableChunkException e)
                        {
                            Logger.LogError(e, "Could not read chunk {0} from region file {1}", c, region);
                            missingChunks.Add(c);
                        }
                    }
                }
            }

            return new QueryResult<CompoundTags>(data, missingChunks);
        }
    }
}
