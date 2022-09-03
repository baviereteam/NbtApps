using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtTools.Entities.Trading
{
    public class TradeFactory
    {
        public static Trade FromTradeTag(CompoundTag rootTag) {
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

        public static TradeComponent TradeComponentFromTag(CompoundTag rootTag)
        {
            try
            {
                var id = (rootTag["id"] as StringTag).Value;
                if (id == "minecraft:air")
                {
                    return null;
                }

                var count = (rootTag["Count"] as ByteTag).Value;
                var metadata = MetadataFromTag(rootTag["tag"] as CompoundTag);
                return new TradeComponent(id, count, metadata);
            }

            catch (Exception e)
            {
                throw new Exception("Could not create trade component", e);
            }
        }

        public static ICollection<string> MetadataFromTag(CompoundTag rootTag)
        {
            try
            {
                var metadata = new List<string>();

                if (rootTag == null)
                {
                    return metadata;
                }

                var enchantmentsTag = rootTag["Enchantments"] as ListTag;
                var bookEnchantmentsTag = rootTag["StoredEnchantments"] as ListTag;

                if (enchantmentsTag != null)
                {
                    foreach (CompoundTag enchantment in enchantmentsTag)
                    {
                        var id = (enchantment["id"] as StringTag).Value;
                        var lvl = (enchantment["lvl"] as ShortTag).Value;
                        metadata.Add($"{id} {lvl}");
                    }
                }
                else if (bookEnchantmentsTag != null)
                {
                    foreach (CompoundTag enchantment in bookEnchantmentsTag)
                    {
                        var id = (enchantment["id"] as StringTag).Value;
                        var lvl = (enchantment["lvl"] as ShortTag).Value;
                        metadata.Add($"{id} {lvl}");
                    }
                }

                return metadata;
            }
            catch (Exception e)
            {
                throw new Exception("Could not create trade component metadata", e);
            }
        }

        public static ICollection<Trade> FromRecipesTag(ListTag recipesTag)
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
