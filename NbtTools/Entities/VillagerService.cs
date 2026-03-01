using NbtTools.Entities.Trading;
using NbtTools.Geography;
using NbtTools.Items;
using SharpNBT;
using System;
using System.Collections.Generic;
using Trades = System.Collections.Generic.ICollection<NbtTools.Entities.Trading.Trade>;
using Villagers = System.Collections.Generic.ICollection<NbtTools.Entities.Villager>;

namespace NbtTools.Entities
{
    public class VillagerService : NbtService
    {
        private readonly TradeService tradeService;

        public VillagerService(RegionQueryService regionQuery, TradeService tradeService) : base(regionQuery)
        {
            this.tradeService = tradeService;
        }

        public QueryResult<Villagers> GetVillagers(Cuboid zone)
        {
            var dataSource = regionQuery.GetEntitiesDataSource(zone);
            var villagerTags = nbtFilter.GetAllCompoundsWithId(dataSource.Result, "minecraft:villager");
            var villagers = new List<Villager>();

            foreach (var villagerTag in villagerTags)
            {
                var villager = FromNbtTag(villagerTag);
                if (villager.Position.ContainedIn(zone))
                {
                    villagers.Add(villager);
                }
            }

            return new QueryResult<Villagers>(villagers, dataSource.UnreadableChunks);
        }

        public QueryResult<Trades> GetTradesFor(Cuboid zone, Searchable searchedItem) 
        {
            var dataSource = regionQuery.GetEntitiesDataSource(zone);
            var villagerTags = nbtFilter.GetAllCompoundsWithId(dataSource.Result, "minecraft:villager");
            var trades = new List<Trade>();

            foreach (var villagerTag in villagerTags)
            {
                var villager = FromNbtTag(villagerTag);
                if (villager.Position.ContainedIn(zone))
                {
                    foreach (var trade in villager.Trades)
                    {
                        if (TradeMatchesSearch(trade, searchedItem))
                        {
                            trades.Add(trade);
                        }
                    }
                }
            }

            return new QueryResult<Trades>(trades, dataSource.UnreadableChunks);
        }

        private bool TradeMatchesSearch(Trade trade, Searchable searchedItem)
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

        private Villager FromNbtTag(Versioned<CompoundTag> versionedRootTag)
        {
            try
            {
                var rootTag = versionedRootTag.Tag;
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
                    Versioned<ListTag> recipes = versionedRootTag
                        .Get<CompoundTag>("Offers")
                        .Get<ListTag>("Recipes");
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
