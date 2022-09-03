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

            using (var stream = new MemoryStream(chunk.Data))
            {
                using (var uncompressor = new SharpNBT.ZLib.ZLibStream(stream, CompressionMode.Decompress))
                {
                    using (var reader = new TagReader(uncompressor, FormatOptions.Java, false))
                    {
                        rootTag = reader.ReadTag<CompoundTag>();
                    }
                }
            }

            if (rootTag == null)
            {
                throw new Exception($"Unreadable NBT.");
            }
            else
            {
                return rootTag;
            }
        }
    }
}
