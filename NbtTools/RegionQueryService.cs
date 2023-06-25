using NbtTools.Geography;
using NbtTools.Mca;
using NbtTools.Nbt;
using SharpNBT;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools
{
    public class RegionQueryService
    {
        private readonly NbtReader reader = new NbtReader();
        private readonly McaFileFactory McaFileFactory;

        public RegionQueryService(McaFileFactory mcaFileFactory)
        {
            McaFileFactory = mcaFileFactory;
        }

        public ICollection<CompoundTag> GetEntitiesDataSource(Cuboid zone)
        {
            var chunks = zone.GetAllChunks();
            var regions = chunks.Select(c => c.Region).Distinct();

            var data = new List<CompoundTag>();

            foreach (var region in regions)
            {
                var file = McaFileFactory.GetEntitiesFile(zone.Dimension, region.GetFileName());
                var regionChunks = chunks.Where(c => c.Region.Equals(region));

                foreach (Chunk c in regionChunks)
                {
                    var chunk = file.GetChunk(c.GetChunkId());
                    if (chunk.Length > 0)
                    {
                        var chunkMainTag = reader.ReadChunk(chunk);
                        var chunkEntitiesCollection = chunkMainTag["Entities"] as ListTag;
                        if (chunkEntitiesCollection != null)
                        {
                            foreach (var entity in chunkEntitiesCollection)
                            {
                                data.Add((CompoundTag)entity);
                            }
                        }
                    }
                }
            }

            return data;
        }
        
        public ICollection<CompoundTag> GetBlockEntitiesDataSource(Cuboid zone, bool includeProtoChunks)
        {
            var chunks = zone.GetAllChunks();
            var regions = chunks.Select(c => c.Region).Distinct();

            var data = new List<CompoundTag>();

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
                        var chunkMainTag = reader.ReadChunk(chunk);
                        var status = chunkMainTag["Status"] as StringTag;
                        if (status != null && status == "minecraft:full")
                        {
                            var blockEntities = chunkMainTag["block_entities"] as ListTag;
                            foreach (var blockEntity in blockEntities)
                            {
                                data.Add(blockEntity as CompoundTag);
                            }

                        }
                    }
                }
            }

            return data;
        }
    }
}
