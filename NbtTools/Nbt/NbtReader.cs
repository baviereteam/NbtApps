using NbtTools.Mca;
using SharpNBT;
using System;
using System.IO;
using System.IO.Compression;

namespace NbtTools.Nbt
{
    public class NbtReader
    {
        public CompoundTag ReadChunk(ChunkEntry chunk)
        {
            CompoundTag rootTag = null;

            try
            {
                using (var stream = new MemoryStream(chunk.Data))
                {
                    using (var uncompressor = new ZLibStream(stream, CompressionMode.Decompress))
                    {
                        using (var reader = new TagReader(uncompressor, FormatOptions.Java, false))
                        {
                            rootTag = reader.ReadTag<CompoundTag>();
                        }
                    }
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
