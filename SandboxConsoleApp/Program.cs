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

            var task = new ItemsDatabaseSeeder();
            task.Run();

            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }
    }
}
