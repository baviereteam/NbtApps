using NbtTools.Entities.Trading;
using SharpNBT;
using System.Collections.Generic;
using System;

namespace NbtTools.Entities.Providers
{
    public class EntityReader
    {
        public virtual int GetCountFromItemTag(CompoundTag tag)
        {
            return (tag["Count"] as ByteTag).Value;
        }

        public virtual ICollection<Enchantment> GetEnchantmentsFromTradeComponent(CompoundTag tradeComponentTag)
        {
            try
            {
                var enchantments = new List<Enchantment>();

                if (!tradeComponentTag.ContainsKey("tag"))
                {
                    return enchantments;
                }

                var metadataTag = tradeComponentTag["tag"] as CompoundTag;

                if (metadataTag.ContainsKey("Enchantments"))
                {
                    var enchantmentsTag = metadataTag["Enchantments"] as ListTag;
                    foreach (CompoundTag enchantment in enchantmentsTag)
                    {
                        var id = (enchantment["id"] as StringTag).Value;
                        var lvl = (enchantment["lvl"] as ShortTag).Value;
                        enchantments.Add(new Enchantment(id, lvl));
                    }
                }
                if (metadataTag.ContainsKey("StoredEnchantments"))
                {
                    var bookEnchantmentsTag = metadataTag["StoredEnchantments"] as ListTag;
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
    }
}
