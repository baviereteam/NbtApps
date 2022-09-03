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

        public ICollection<CompoundTag> GetEntitiesDataSource(Cuboid zone)
        {
            var chunks = zone.GetAllChunks();
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

            var data = new List<CompoundTag>();

            var file = new McaFile($@"C:\Users\Cycy\Downloads\map\home\minecraft\server\partywoop\entities\{region}");
            var headers = file.GetHeader();

            foreach (Chunk c in chunks)
            {
                var chunk = file.GetChunk(c.GetChunkId());
                if (chunk.Length > 0)
                {
                    var chunkMainTag = reader.ReadChunk(chunk);
                    var chunkEntitiesCollection = chunkMainTag["Entities"];
                    if (chunkEntitiesCollection != null && chunkEntitiesCollection is ListTag)
                    {
                        foreach (var entity in (ListTag) chunkEntitiesCollection)
                        {
                            data.Add((CompoundTag) entity);
                        }
                    }
                }
            }

            return data;
        }
    }
}
