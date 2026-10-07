using NbtTools.Entities.Trading;
using fNbt;
using System.Collections.Generic;
using System;

namespace NbtTools.Entities.Providers
{
    public class EntityReader
    {
        public virtual int GetCountFromItemTag(NbtCompound tag)
        {
            return (tag["Count"] as NbtByte).Value;
        }

        public virtual ICollection<Enchantment> GetEnchantmentsFromTradeComponent(NbtCompound tradeComponentTag)
        {
            try
            {
                var enchantments = new List<Enchantment>();

                if (!tradeComponentTag.ContainsKey("tag"))
                {
                    return enchantments;
                }

                var metadataTag = tradeComponentTag["tag"] as NbtCompound;

                if (metadataTag.ContainsKey("Enchantments"))
                {
                    var enchantmentsTag = metadataTag["Enchantments"] as NbtList;
                    foreach (NbtTag t in enchantmentsTag)
                    {
                        if (t is NbtCompound enchantment)
                        {
                            var id = (enchantment["id"] as NbtString).Value;
                            var lvl = (enchantment["lvl"] as NbtShort).Value;
                            enchantments.Add(new Enchantment(id, lvl));
                        }
                    }
                }
                if (metadataTag.ContainsKey("StoredEnchantments"))
                {
                    var bookEnchantmentsTag = metadataTag["StoredEnchantments"] as NbtList;
                    foreach (NbtTag t in bookEnchantmentsTag)
                    {
                        if (t is NbtCompound enchantment)
                        {
                            var id = (enchantment["id"] as NbtString).Value;
                            var lvl = (enchantment["lvl"] as NbtShort).Value;
                            enchantments.Add(new Enchantment(id, lvl));
                        }
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
