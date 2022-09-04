using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.IO;
using NbtTools.Geography;
using NbtTools.Mca;
using SharpNBT;
using NbtTools.Nbt;
using NbtTools.Entities;
using System.Text;

namespace SandboxConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press ENTER to start.");
            Console.ReadLine();

            var startPoint = new Point(-310, 65, -2085);
            var endPoint = new Point(-272, 78, -2123);

            Console.Write("Start point: ");
            Console.WriteLine(startPoint);

            Console.Write("End point: ");
            Console.WriteLine(endPoint);

            Cuboid cuboid = new Cuboid(startPoint, endPoint);

            Console.WriteLine();

            var villagerService = new VillagerService();
            var villagers = villagerService.GetVillagers(cuboid);

            foreach (var villager in villagers)
            {
                Console.WriteLine($"Villager: {villager.Job} ({villager.Level}) at {villager.Position}");
                foreach (var trade in villager.Trades)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"\tSells {trade.Sell.Quantity} of {trade.Sell.Item}");
                    
                    if (trade.Sell.Metadata.Count > 0)
                    {
                        sb.Append(" (with metadata)");
                    }

                    sb.Append($" for {trade.Buy1.Quantity} of {trade.Buy1.Item}");

                    if (trade.Buy1.Metadata.Count > 0)
                    {
                        sb.Append(" (with metadata)");
                    }

                    if (trade.Buy2 != null)
                    {
                        sb.Append($" and {trade.Buy2.Quantity} of {trade.Buy2.Item}");

                        if (trade.Buy2.Metadata.Count > 0)
                        {
                            sb.Append(" (with metadata)");
                        }
                    }

                    Console.WriteLine(sb.ToString());
                }
            }

            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }
    }
}
