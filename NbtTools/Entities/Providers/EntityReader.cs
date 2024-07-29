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
                var metadataTag = tradeComponentTag["tag"] as CompoundTag;
                var enchantments = new List<Enchantment>();

                if (metadataTag == null)
                {
                    return enchantments;
                }

                var enchantmentsTag = metadataTag["Enchantments"] as ListTag;
                var bookEnchantmentsTag = metadataTag["StoredEnchantments"] as ListTag;

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
    }
}
