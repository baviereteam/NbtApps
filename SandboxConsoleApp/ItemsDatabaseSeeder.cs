using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NbtTools.Database;
using NbtTools.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SandboxConsoleApp
{
    internal class ItemsDatabaseSeeder
    {
        private NbtDbContext context;
        public void Run()
        {
            var options = new DbContextOptionsBuilder<NbtDbContext>()
            .UseSqlite(
                "Filename=C:\\Users\\Cycy\\Source\\Repos\\NbtApps\\NbtTools\\Database\\nbt.db",
                sqLiteOptions => sqLiteOptions.MigrationsAssembly("NbtTools")
            )
            .Options;
            context = new NbtDbContext(options);

            string fileName = "C:\\Users\\Cycy\\Desktop\\MC\\blocksanditems-cleaned.json";
            string jsonString = File.ReadAllText(fileName);

            var itemsWithoutDisplayName = new List<string>();

            int i = 0;

            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                var root = document.RootElement;
                var items = root.GetProperty("items");
                foreach(var item in items.EnumerateObject().Select(node => node.Value))
                {
                    var id = item.GetProperty("text_id").GetString();
                    var name = item.GetProperty("display_name").GetString();
                    var stackSize = item.GetProperty("max_stack_size").GetByte();
                    context.Items.Add(new Item(id, name, stackSize));
                    i++;
                }

                context.SaveChanges();
                Console.WriteLine($"Added {i} items.");
            }
        }
    }
}
