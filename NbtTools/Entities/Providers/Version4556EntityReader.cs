using NbtTools.Entities.Trading;
using fNbt;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities.Providers
{
    // 1.21.10
    internal class Version4556EntityReader : Version3837EntityReader
    {
        public override ICollection<Enchantment> GetEnchantmentsFromTradeComponent(NbtCompound tradeComponentTag)
        {
            try
            {
                var enchantments = new List<Enchantment>();

                if (!tradeComponentTag.ContainsKey("components"))
                {
                    return enchantments;
                }
                var componentsTag = tradeComponentTag["components"] as NbtCompound;


                if (componentsTag.ContainsKey("minecraft:enchantments"))
                {
                    var enchantmentsTag = componentsTag["minecraft:enchantments"] as NbtCompound;
                    foreach (NbtTag tag in enchantmentsTag)
                    {
                        if (tag is NbtInt enchantmentTag)
                        {
                            enchantments.Add(new Enchantment(enchantmentTag.Name, enchantmentTag.Value));
                        }
                    }
                }

                if (componentsTag.ContainsKey("minecraft:stored_enchantments"))
                {
                    var bookEnchantmentsTag = componentsTag["minecraft:stored_enchantments"] as NbtCompound;
                    foreach (NbtTag tag in bookEnchantmentsTag)
                    {
                        if (tag is NbtInt enchantmentTag)
                        {
                            enchantments.Add(new Enchantment(enchantmentTag.Name, enchantmentTag.Value));
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
