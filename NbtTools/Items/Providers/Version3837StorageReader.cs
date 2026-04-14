using SharpNBT;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Items.Providers
{
    // 1.20.5
    // https://misode.github.io/versions/?id=1.20.5&tab=changelog
    internal class Version3837StorageReader : StorageReader
    {
        protected override int GetCountFromItemTag(CompoundTag itemTag)
        {
            //Renamed "Count" → "count".The count now defaults to 1 and will not be present in that case.
            var countTag = itemTag["count"];

            if (countTag == null)
            {
                return 1;
            }

            return (countTag as IntTag).Value;
        }

        /// <summary>
        /// Counts the items in a shulker box that is itself an item in a container.
        /// </summary>
        /// <param name="shulkerBox"></param>
        /// <param name="searchedItem"></param>
        /// <returns></returns>
        internal override IDictionary<Searchable, int> CountItemsInContainedShulkerBox(CompoundTag shulkerBox, ICollection<Searchable> searchedItems)
        {
            var results = new Dictionary<Searchable, int>();

            if (!shulkerBox.ContainsKey("components"))
            {
                return results;
            }

            var componentsTag = shulkerBox["components"] as CompoundTag;

            // Empty shulker boxes don't have a "minecraft:container".
            if (!componentsTag.ContainsKey("minecraft:container"))
            {
                return results;
            }

            // List of compound (slot,item)
            // where item is a compound (id, count)
            var containerContents = componentsTag["minecraft:container"] as ListTag;
            if (containerContents == null)
            {
                return results;
            }

            foreach (var slot in containerContents)
            {
                var slotTag = slot as CompoundTag;
                var itemTag = slotTag["item"] as CompoundTag;

                var searchableThatMatchesThisItem = searchedItems.SingleOrDefault(searchable => ItemTagIs(itemTag, searchable), null);
                if (searchableThatMatchesThisItem == null)
                {
                    continue;
                }

                results.AddOrIncrement(searchableThatMatchesThisItem, GetCountFromItemTag(itemTag));
            }

            return results;
        }

        /// <summary>
        /// Indicates whether the provided item tag is a potion matching the search.
        /// </summary>
        /// <param name="itemTag"></param>
        /// <param name="searchedPotion"></param>
        /// <returns></returns>
        protected override bool IsMatchingPotion(CompoundTag itemTag, Potion searchedPotion)
        {
            var componentsTag = itemTag["components"] as CompoundTag;
            if (componentsTag == null)
            {
                return false;
            }

            var potionContentsTag = componentsTag["minecraft:potion_contents"] as CompoundTag;
            if (potionContentsTag == null)
            {
                return false;
            }

            var potionTag = potionContentsTag["potion"] as StringTag;
            if (potionTag == null)
            {
                return false;
            }

            return potionTag == searchedPotion.PotionContents;
        }

        /// <summary>
        /// Indicates whether the provided item tag is an enchanted book matching the search.
        /// </summary>
        /// <param name="itemTag"></param>
        /// <param name="searchedBook"></param>
        /// <returns></returns>
        protected override bool IsMatchingEnchantedBook(CompoundTag itemTag, EnchantedBook searchedBook)
        {
            var componentsTag = itemTag["components"] as CompoundTag;
            if (componentsTag == null)
            {
                return false;
            }

            var storedEnchantmentsTag = componentsTag["minecraft:stored_enchantments"] as CompoundTag;
            if (storedEnchantmentsTag == null)
            {
                return false;
            }

            var levelsTag = storedEnchantmentsTag["levels"] as CompoundTag;
            if (levelsTag == null || !levelsTag.ContainsKey(searchedBook.Enchantment))
            {
                return false;
            }

            return (levelsTag[searchedBook.Enchantment] as IntTag).Value == searchedBook.Level;
        }
    }
}