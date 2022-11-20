using NbtTools.Geography;
using NbtTools.Items;
using NbtTools.Mca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxConsoleApp
{
    internal class ItemSearchTester
    {
        public void Run()
        {
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
        }
    }
}
