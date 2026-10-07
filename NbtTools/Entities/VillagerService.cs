using NbtTools.Entities.Trading;
using NbtTools.Geography;
using NbtTools.Items;
using NbtTools.Nbt;
using NbtTools.RegionQuery;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Entities
{
    public class VillagerService
    {
        private readonly EntitiesQuery RegionQuery;
        private readonly TradeService TradeService;

        public VillagerService(EntitiesQuery regionQuery, TradeService tradeService)
        {
            this.TradeService = tradeService;
            this.RegionQuery = regionQuery;
        }

        public QueryResult<Villager> GetVillagers(Cuboid zone)
        {
            var dataSource = RegionQuery.GetData(zone);
            var villagerTags = NbtFilter.GetAllCompoundsWithId(dataSource.Result, "minecraft:villager");
            var villagers = new List<Villager>();

            foreach (var villagerTag in villagerTags)
            {
                var villager = FromNbtTag(villagerTag);
                if (villager.Position.ContainedIn(zone))
                {
                    villagers.Add(villager);
                }
            }

            return new QueryResult<Villager>(villagers, dataSource.UnreadableChunks);
        }

        public CuboidTradesSearchResult GetTradesFor(Cuboid zone, ICollection<Searchable> searchedItems) 
        {
            var dataSource = RegionQuery.GetData(zone);
            var villagerTags = NbtFilter.GetAllCompoundsWithId(dataSource.Result, "minecraft:villager");
            var results = new CuboidTradesSearchResult();
            results.UnreadableChunks = dataSource.UnreadableChunks;

            foreach (var villagerTag in villagerTags)
            {
                var villager = FromNbtTag(villagerTag);
                if (villager.Position.ContainedIn(zone))
                {
                    foreach (var trade in villager.Trades)
                    {
                        foreach (var search in searchedItems.Where(search => TradeMatchesSearch(trade, search)))
                        {
                            results.Add(search, trade);
                        }
                    }
                }
            }

            return results;
        }

        private static bool TradeMatchesSearch(Trade trade, Searchable searchedItem)
        {
            switch (searchedItem)
            {
                case Item _:
                    return trade.Sell.Item.Id == searchedItem.Id;

                case EnchantedBook book:
                    var searchedEnchantment = new Enchantment(book.Enchantment, book.Level);
                    return 
                        trade.Sell.Item.Id == EnchantedBook.GENERIC_ENCHANTED_BOOK_ID
                        && trade.Sell.Enchantments.Contains(searchedEnchantment);

                // No villager sells potions.
                // case Potion potion:

                default:
                    return false;
            }
        }

        public static IDictionary<string, ICollection<Villager>> OrderByJob(ICollection<Villager> source)
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

        private Villager FromNbtTag(Versioned<NbtCompound> versionedRootTag)
        {
            try
            {
                var rootTag = versionedRootTag.Tag;
                NbtList positionTag = rootTag["Pos"] as NbtList;
                double x = (positionTag[0] as NbtDouble).Value;
                double y = (positionTag[1] as NbtDouble).Value;
                double z = (positionTag[2] as NbtDouble).Value;
                Point position = new Point(x, y, z);

                NbtCompound villagerDataTag = rootTag["VillagerData"] as NbtCompound;
                int level = (villagerDataTag["level"] as NbtInt).Value;
                string profession = (villagerDataTag["profession"] as NbtString).Value;
                string type = (villagerDataTag["type"] as NbtString).Value;
                var villager = new Villager(profession, level, type, position);

                ICollection<Trade> trades;
                if (profession != "minecraft:none" && profession != "minecraft:nitwit")
                {
                    Versioned<NbtList> recipes = versionedRootTag
                        .Get<NbtCompound>("Offers")
                        .Get<NbtList>("Recipes");
                    trades = TradeService.FromRecipesTag(villager, recipes);
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
