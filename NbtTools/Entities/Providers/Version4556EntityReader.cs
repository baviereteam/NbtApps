using NbtTools.Entities.Trading;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities.Providers
{
    // 1.21.10
    internal class Version4556EntityReader : Version3837EntityReader
    {
        public override ICollection<Enchantment> GetEnchantmentsFromTradeComponent(CompoundTag tradeComponentTag)
        {
            try
            {
                var enchantments = new List<Enchantment>();

                if (!tradeComponentTag.ContainsKey("components"))
                {
                    return enchantments;
                }
                var componentsTag = tradeComponentTag["components"] as CompoundTag;


                if (componentsTag.ContainsKey("minecraft:enchantments"))
                {
                    var enchantmentsTag = componentsTag["minecraft:enchantments"] as CompoundTag;
                    foreach (IntTag enchantmentTag in enchantmentsTag)
                    {
                        enchantments.Add(new Enchantment(enchantmentTag.Name, enchantmentTag.Value));
                    }
                }

                if (componentsTag.ContainsKey("minecraft:stored_enchantments"))
                {
                    var bookEnchantmentsTag = componentsTag["minecraft:stored_enchantments"] as CompoundTag;
                    foreach (IntTag enchantmentTag in bookEnchantmentsTag)
                    {
                        enchantments.Add(new Enchantment(enchantmentTag.Name, enchantmentTag.Value));
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
