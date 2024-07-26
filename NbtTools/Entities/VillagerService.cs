using NbtTools.Entities.Trading;
using NbtTools.Geography;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities
{
    public class VillagerService : NbtService
    {
        private readonly TradeService tradeService;

        public VillagerService(RegionQueryService regionQuery, TradeService tradeService) : base(regionQuery)
        {
            this.tradeService = tradeService;
        }

        public ICollection<Villager> GetVillagers(Cuboid zone)
        {
            var dataSource = regionQuery.GetEntitiesDataSource(zone);
            var villagerTags = nbtFilter.GetAllCompoundsWithId(dataSource, "minecraft:villager");
            var villagers = new List<Villager>();

            foreach (var entry in villagerTags)
            {
                var villagerTag = entry.Tag;
                var villager = FromNbtTag(villagerTag);
                if (villager.Position.ContainedIn(zone))
                {
                    villagers.Add(villager);
                }
            }

            return villagers;
        }

        public ICollection<Trade> GetTradesFor(Cuboid zone, string id) 
        {
            var dataSource = regionQuery.GetEntitiesDataSource(zone);
            var villagerTags = nbtFilter.GetAllCompoundsWithId(dataSource, "minecraft:villager");
            var trades = new List<Trade>();

            foreach (var entry in villagerTags)
            {
                var villagerTag = entry.Tag;
                var villager = FromNbtTag(villagerTag);
                if (villager.Position.ContainedIn(zone))
                {
                    foreach (var trade in villager.Trades)
                    {
                        if (trade.Sell.Item.Id == id)
                        {
                            trades.Add(trade);
                        }
                    }
                }
            }

            return trades;
        }

        public IDictionary<string, ICollection<Villager>> OrderByJob(ICollection<Villager> source)
        {
            var destination = new Dictionary<string, ICollection<Villager>>();

            foreach (var villager in source)
            {
                if (!destination.ContainsKey(villager.Job))
                {
                    destination[villager.Job] = new List<Villager>();
                }

                destination[villager.Job].Add(villager);
            }

            return destination;
        }

        private Villager FromNbtTag(CompoundTag rootTag)
        {
            try
            {
                ListTag positionTag = rootTag["Pos"] as ListTag;
                double x = (positionTag[0] as DoubleTag).Value;
                double y = (positionTag[1] as DoubleTag).Value;
                double z = (positionTag[2] as DoubleTag).Value;
                Point position = new Point(x, y, z);

                CompoundTag villagerDataTag = rootTag["VillagerData"] as CompoundTag;
                int level = (villagerDataTag["level"] as IntTag).Value;
                string profession = (villagerDataTag["profession"] as StringTag).Value;
                string type = (villagerDataTag["type"] as StringTag).Value;
                var villager = new Villager(profession, level, type, position);

                ICollection<Trade> trades;
                if (profession != "minecraft:none" && profession != "minecraft:nitwit")
                {
                    ListTag recipes = (rootTag["Offers"] as CompoundTag)["Recipes"] as ListTag;
                    trades = tradeService.FromRecipesTag(villager, recipes);
                }
                else
                {
                    trades = new List<Trade>();
                }

                villager.Trades = trades;
                return villager;
            }

            catch (Exception e)
            {
                throw new Exception("Could not create villager", e);
            }
        }
    }
}
