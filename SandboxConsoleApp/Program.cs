using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.IO;
using NbtTools.Geography;
using NbtTools.Mca;
using SharpNBT;

namespace SandboxConsoleApp
{
    class Program
    {
        private static IDictionary<int, HeaderEntry> headers;
        private static McaFile file;

        static void Main(string[] args)
        {
            var startPoint = new Point(-310, 65, -2085);
            var endPoint = new Point(-272, 78, -2123);

            Console.Write("Start point: ");
            Console.WriteLine(startPoint);

            Console.Write("End point: ");
            Console.WriteLine(endPoint);

            Cuboid cuboid = new Cuboid(startPoint, endPoint);
            var chunks = cuboid.GetAllChunks();

            Console.WriteLine("\nChunks:");
            Region region = null;
            foreach(Chunk c in chunks)
            {
                Console.WriteLine(c);

                // Verifies all chunks are in the same region
                if (region == null)
                {
                    region = c.Region;
                }
                if (!region.Equals(c.Region))
                {
                    throw new NotImplementedException($"This shop zone extends on multiple Minecraft regions ({region}, {c.Region}). This is not supported yet.");
                }
            }

            Console.WriteLine($"\nRegion: {region}");

            file = new McaFile($@"C:\Users\Cycy\Downloads\map\home\minecraft\server\partywoop\entities\{region}");
            headers = file.GetHeader();

            Console.WriteLine($"Region file size (bytes): {file.Length}");
            Console.WriteLine("Region headers:");
            for (int i = 0; i < McaFile.HEADER_ENTRIES; i++)
            {
                if (!headers[i].IsEmpty())
                {
                    Console.WriteLine(headers[i]);
                    ChunkEntry chunk = file.GetChunk(i);
                    Console.WriteLine($"\tLength: {chunk.Length}, Compression type: {chunk.CompressionType}");
                    Console.WriteLine($"\tCoords: {ReadChunkCoords(i)}");
                }
            }
            
            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }

        private static Point ReadChunkCoords(int chunkId)
        {
            ChunkEntry chunk = file.GetChunk(chunkId);
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
                throw new Exception($"Unreadable NBT for chunk {chunkId}.");
            }

            try
            {
                IntArrayTag position = rootTag.Find("Position", false) as IntArrayTag;
                return new Point(position[0], 0, position[1]);
            }
            catch (Exception e)
            {
                throw new Exception($"Could not find coordinates inside chunk {chunkId}.", e);
            }
        }
    }
}
