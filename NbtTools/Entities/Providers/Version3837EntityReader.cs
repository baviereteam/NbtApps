using NbtTools.Entities.Trading;
using fNbt;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities.Providers
{
    // 1.20.5
    // https://misode.github.io/versions/?id=1.20.5&tab=changelog
    internal class Version3837EntityReader : EntityReader
    {
        public override int GetCountFromItemTag(NbtCompound tag)
        {
            //Renamed "Count" → "count".The count now defaults to 1 and will not be present in that case.
            var countTag = tag["count"];

            if (countTag == null)
            {
                return 1;
            }

            return (countTag as NbtInt).Value;
        }

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

                    var levelsTag = enchantmentsTag["levels"] as NbtCompound;
                    foreach (NbtInt enchantmentTag in levelsTag)
                    {
                        enchantments.Add(new Enchantment(enchantmentTag.Name, enchantmentTag.Value));
                    }
                }

                if (componentsTag.ContainsKey("minecraft:stored_enchantments"))
                {
                    var bookEnchantmentsTag = componentsTag["minecraft:stored_enchantments"] as NbtCompound;
                    var levelsTag = bookEnchantmentsTag["levels"] as NbtCompound;
                    foreach (NbtInt enchantmentTag in levelsTag)
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
