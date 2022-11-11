using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.IO;
using NbtTools.Geography;
using NbtTools.Mca;
using NbtTools.Nbt;
using NbtTools.Entities;
using System.Text;
using NbtTools.Items;
using System.Linq;

namespace SandboxConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press ENTER to start.");
            Console.ReadLine();

            var startPoint = new Point(-248, 76, -2164);
            var endPoint = new Point(-247, 74, -2164);

            Console.Write("Start point: ");
            Console.WriteLine(startPoint);

            Console.Write("End point: ");
            Console.WriteLine(endPoint);

            Cuboid cuboid = new Cuboid(startPoint, endPoint);

            Console.WriteLine();

            McaFile.RootPath = @"C:\Users\Cycy\Downloads\map\home\minecraft\server\partywoop\region";

            var searchService = new StoredItemService();
            var results = searchService.FindStoredItems("minecraft:cobblestone", cuboid);

            foreach (var storeditem in results)
            {
                Console.WriteLine(storeditem);
            }

            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }
    }
}
