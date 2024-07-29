using NbtTools.Database;
using NbtTools.Entities.Providers;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities.Trading
{
    public class TradeService : NbtService
    {
        private readonly NbtDbContext NbtContext;
        private readonly EntityReaderFactory EntityReaderFactory;

        public TradeService(RegionQueryService regionQuery, NbtDbContext context, EntityReaderFactory entityReaderFactory) : base(regionQuery)
        {
            NbtContext = context;
            EntityReaderFactory = entityReaderFactory;
        }

        public ICollection<Trade> FromRecipesTag(Villager villager, Versioned<ListTag> recipesTag)
        {
            var trades = new List<Trade>();

            foreach (var recipe in recipesTag.Enumerate())
            {
                trades.Add(FromTradeTag(villager, recipe.As<CompoundTag>()));
            }

            return trades;
        }

        public Trade FromTradeTag(Villager villager, Versioned<CompoundTag> versionedRootTag) {
            try
            {
                var buy1 = TradeComponentFromTag(versionedRootTag.Get<CompoundTag>("buy"));
                var buy2 = TradeComponentFromTag(versionedRootTag.Get<CompoundTag>("buyB"));
                var sell = TradeComponentFromTag(versionedRootTag.Get<CompoundTag>("sell"));

                return new Trade(villager, buy1, buy2, sell);
            }

            catch (Exception e)
            {
                throw new Exception("Could not create trade", e);
            }
        }

        public TradeComponent? TradeComponentFromTag(Versioned<CompoundTag> versionedRootTag)
        {
            // "buy2" is missing when the trade is one item vs. one item.
            if (versionedRootTag.Tag == null)
            {
                return null;
            }

            try
            {
                var id = (versionedRootTag.Tag["id"] as StringTag).Value;
                if (id == "minecraft:air")
                {
                    return null;
                }

                var item = NbtContext.Items.Find(id);
                if (item == null)
                {
                    throw new KeyNotFoundException($"Item {id} did not exist in the NBT database.");
                }

                var entityReader = EntityReaderFactory.GetForVersion(versionedRootTag.DataVersion);
                var count = entityReader.GetCountFromItemTag(versionedRootTag.Tag);
                var enchantments = entityReader.GetEnchantmentsFromTradeComponent(versionedRootTag.Tag);
                return new TradeComponent(item, count, enchantments);
            }

            catch (Exception e)
            {
                throw new Exception("Could not create trade component", e);
            }
        }
    }
}
