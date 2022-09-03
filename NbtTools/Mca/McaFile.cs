using System;
using System.Collections.Generic;
using System.IO;

namespace NbtTools.Mca
{
    public class McaFile
    {
        public const int HEADER_ENTRIES = 1024;

        public string Path { get; private set; }
        private IDictionary<long, HeaderEntry> header;
        private IDictionary<long, ChunkEntry> chunks;
        public long Length { 
            get 
            {
                return new FileInfo(Path).Length;
            }
        }

        public McaFile(string path)
        {
            Path = path;
            header = null;
            chunks = new Dictionary<long, ChunkEntry>();
        }

        public IDictionary<long, HeaderEntry> GetHeader()
        {
            if (header == null)
            {
                ReadHeader();
            }

            return header;
        }

        public ChunkEntry GetChunk(long id)
        {
            if (!chunks.ContainsKey(id))
            {
                ReadChunk(id);
            }

            return chunks[id];
        }

        private void ReadHeader()
        {
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException($"File {Path} was not found.");
            }

            header = new Dictionary<long, HeaderEntry>();

            using (var stream = new FileStream(Path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    // Parse locations (1024 entries; 4 bytes each)
                    // 3 first bytes: unsigned big endian (offset)
                    // 4th byte: length of chunk
                    for (int i = 0; i < HEADER_ENTRIES; i++)
                    {
                        long currentPos = reader.BaseStream.Position;
                        HeaderEntry entry = new HeaderEntry(i);
                        entry.Offset = BytesToInt(reader.ReadBytes(3)) * 4096; // value from the file is a 4KiB offset.
                        entry.Length = reader.ReadByte();

                        header.Add(currentPos, entry);
                    }

                    // Parse timestamps (1024 entries: 4-byte big-endian integers; last chunk modification timestamp)
                    for (int i = 0; i < HEADER_ENTRIES; i++)
                    {
                        header[i * 4].Timestamp = BytesToInt(reader.ReadBytes(4));
                    }

                    Console.Write($"Header finished at position {reader.BaseStream.Position} bytes.");
                }
            }
        }

        private void ReadChunk(long id)
        {
            if (header == null)
            {
                ReadHeader();
            }

            if (!header.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException($"Chunk with ID {id} does not exist.");
            }

            var chunk = new ChunkEntry(id);

            using (var stream = new FileStream(Path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    // find the chunk in the stream
                    reader.BaseStream.Position = header[id].Offset;

                    // read data
                    chunk.Length = BytesToInt(reader.ReadBytes(4)); // total length = compression type (1 byte) + data (length-1 bytes)
                    chunk.CompressionType = reader.ReadByte();
                    if (chunk.Length > 0)
                    {
                        chunk.Data = reader.ReadBytes(chunk.Length - 1);
                    }
                }
            }

            chunks.Add(id, chunk);
        }

        private int BytesToInt(byte[] bytes, bool isBigEndian = true)
        {
            // BitConverter.IsLittleEndian = true on all x86-64s
            if (isBigEndian)
            {
                Array.Reverse(bytes);
            }

            // int = 4 bytes, do we need to pad?
            // LittleEndian target: pad on the right (higher-order bits)
            if (bytes.Length < 4)
            {
                var newBytes = new byte[4];
                Array.Copy(bytes, 0, newBytes, 0, bytes.Length);
                bytes = newBytes;
            }

            int result = BitConverter.ToInt32(bytes, 0);
            return result;
        }
    }
}
