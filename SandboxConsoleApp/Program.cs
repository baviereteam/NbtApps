using System;
using System.Linq;
using NbtTools;

namespace SandboxConsoleApp
{
    class Program
    {
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

            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }
    }
}
