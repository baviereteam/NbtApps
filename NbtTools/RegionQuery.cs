using NbtTools.Geography;
using NbtTools.Mca;
using NbtTools.Nbt;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools
{
    public class RegionQuery
    {
        private NbtReader reader = new NbtReader();

        private Region getRegionFromChunks(ICollection<Chunk> chunks)
        {
            Region region = null;

            foreach (Chunk c in chunks)
            {
                // Verifies all chunks are in the same region
                if (region == null)
                {
                    region = c.Region;
                }
                if (!region.Equals(c.Region))
                {
                    //TODO: support multi-region
                    throw new NotImplementedException($"This zone extends on multiple Minecraft regions ({region}, {c.Region}). This is not supported yet.");
                }
            }

            return region;
        }

        public ICollection<CompoundTag> GetEntitiesDataSource(Cuboid zone)
        {
            var chunks = zone.GetAllChunks();
            var region = getRegionFromChunks(chunks);

            var data = new List<CompoundTag>();

            var file = new McaFile(region.GetFileName());

            foreach (Chunk c in chunks)
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
                            data.Add((CompoundTag) entity);
                        }
                    }
                }
            }

            return data;
        }
        
        public ICollection<CompoundTag> GetBlockEntitiesDataSource(Cuboid zone, bool includeProtoChunks)
        {
            var chunks = zone.GetAllChunks();
            var region = getRegionFromChunks(chunks);

            var data = new List<CompoundTag>();

            var file = new McaFile(region.GetFileName());
            var headers = file.GetHeader();

            foreach (Chunk c in chunks)
            {
                var chunk = file.GetChunk(c.GetChunkId());
                if (chunk.Length > 0)
                {
                    var chunkMainTag = reader.ReadChunk(chunk);
                    var status = chunkMainTag["Status"] as StringTag;
                    if (status != null && status == "full")
                    {
                        var blockEntities = chunkMainTag["block_entities"] as ListTag;
                        foreach (var blockEntity in blockEntities)
                        {
                            data.Add(blockEntity as CompoundTag);
                        }
                        
                    }
                }
            }

            return data;
        }
    }
}
