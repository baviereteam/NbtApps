using fNbt;
using NbtTools.Mca;
using System;
using System.IO;

namespace NbtTools.Nbt
{
    public static class ChunkNbtReader
    {
        public static NbtCompound ReadChunk(ChunkEntry chunk)
        {
            NbtCompound rootTag = null;

            try
            {
                using (var stream = new MemoryStream(chunk.Data))
                {
                    NbtFile file = new NbtFile();
                    file.LoadFromStream(stream, NbtCompression.ZLib);
                    rootTag = file.RootTag;
                }
            }
            catch (Exception e) 
            {
                throw new UnreadableChunkException("Could not read the chunk NBT", e);
            }

            if (rootTag == null)
            {
                throw new UnreadableChunkException("The chunk NBT did not produce a root tag");
            }

            return rootTag;
        }
    }
}
