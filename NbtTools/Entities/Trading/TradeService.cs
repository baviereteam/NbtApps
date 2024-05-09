using NbtTools.Database;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities.Trading
{
    public class TradeService : NbtService
    {
        private readonly NbtDbContext nbtContext;

        public TradeService(RegionQueryService regionQuery, NbtDbContext context) : base(regionQuery)
        {
            nbtContext = context;
        }

        public Trade FromTradeTag(CompoundTag rootTag) {
            try
            {
                var buy1 = TradeComponentFromTag(rootTag["buy"] as CompoundTag);
                var buy2 = TradeComponentFromTag(rootTag["buyB"] as CompoundTag);
                var sell = TradeComponentFromTag(rootTag["sell"] as CompoundTag);

                return new Trade(buy1, buy2, sell);
            }

            catch (Exception e)
            {
                throw new Exception("Could not create trade", e);
            }
        }

        public TradeComponent TradeComponentFromTag(CompoundTag rootTag)
        {
            try
            {
                var id = (rootTag["id"] as StringTag).Value;
                if (id == "minecraft:air")
                {
                    return null;
                }

                //TODO need to retrieve the actual item here
                var item = nbtContext.Items.Find(id);
                if (item == null)
                {
                    throw new KeyNotFoundException($"Item {id} did not exist in the NBT database.");
                }

                var count = (rootTag["Count"] as ByteTag).Value;

                var metadataTag = rootTag["tag"] as CompoundTag;
                var enchantments = EnchantmentsFromTag(metadataTag);
                return new TradeComponent(item, count, enchantments);
            }

            catch (Exception e)
            {
                throw new Exception("Could not create trade component", e);
            }
        }

        public ICollection<Enchantment> EnchantmentsFromTag(CompoundTag rootTag)
        {
            try
            {
                var enchantments = new List<Enchantment>();

                if (rootTag == null)
                {
                    return enchantments;
                }

                var enchantmentsTag = rootTag["Enchantments"] as ListTag;
                var bookEnchantmentsTag = rootTag["StoredEnchantments"] as ListTag;

                if (enchantmentsTag != null)
                {
                    foreach (CompoundTag enchantment in enchantmentsTag)
                    {
                        var id = (enchantment["id"] as StringTag).Value;
                        var lvl = (enchantment["lvl"] as ShortTag).Value;
                        enchantments.Add(new Enchantment(id, lvl));
                    }
                }
                if (bookEnchantmentsTag != null)
                {
                    foreach (CompoundTag enchantment in bookEnchantmentsTag)
                    {
                        var id = (enchantment["id"] as StringTag).Value;
                        var lvl = (enchantment["lvl"] as ShortTag).Value;
                        enchantments.Add(new Enchantment(id, lvl));
                    }
                }

                return enchantments;
            }
            catch (Exception e)
            {
                throw new Exception("Could not create trade component enchantment metadata", e);
            }
        }

        public ICollection<Trade> FromRecipesTag(ListTag recipesTag)
        {
            var trades = new List<Trade>();

            foreach (var recipe in recipesTag)
            {
                trades.Add(FromTradeTag(recipe as CompoundTag));
            }

            return trades;
        }
    }
}
