using NbtTools.Entities.Trading;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities.Providers
{
    internal class Version3837EntityReader : EntityReader
    {
        public override int GetCountFromItemTag(CompoundTag tag)
        {
            //Renamed "Count" → "count".The count now defaults to 1 and will not be present in that case.
            var countTag = tag["count"];

            if (countTag == null)
            {
                return 1;
            }

            return (countTag as IntTag).Value;
        }

        public override ICollection<Enchantment> GetEnchantmentsFromTradeComponent(CompoundTag tradeComponentTag)
        {
            try
            {
                var componentsTag = tradeComponentTag["components"] as CompoundTag;
                var enchantments = new List<Enchantment>();

                if (componentsTag == null)
                {
                    return enchantments;
                }

                var enchantmentsTag = componentsTag["minecraft:enchantments"] as CompoundTag;
                var bookEnchantmentsTag = componentsTag["minecraft:stored_enchantments"] as CompoundTag;

                if (enchantmentsTag != null)
                {
                    var levelsTag = enchantmentsTag["levels"] as CompoundTag;
                    foreach (IntTag enchantmentTag in levelsTag)
                    {
                        enchantments.Add(new Enchantment(enchantmentTag.Name, enchantmentTag.Value));
                    }
                }

                if (bookEnchantmentsTag != null)
                {
                    var levelsTag = bookEnchantmentsTag["levels"] as CompoundTag;
                    foreach (IntTag enchantmentTag in levelsTag)
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
