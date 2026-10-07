using fNbt;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Items.Providers
{
    // 1.20.5
    // https://misode.github.io/versions/?id=1.20.5&tab=changelog
    internal class Version3837StorageReader : StorageReader
    {
        protected override int GetCountFromItemTag(NbtCompound itemTag)
        {
            //Renamed "Count" → "count".The count now defaults to 1 and will not be present in that case.
            var countTag = itemTag["count"];

            if (countTag == null)
            {
                return 1;
            }

            return (countTag as NbtInt).Value;
        }

        /// <summary>
        /// Counts the items in a shulker box that is itself an item in a container.
        /// </summary>
        /// <param name="shulkerBox"></param>
        /// <param name="searchedItem"></param>
        /// <returns></returns>
        internal override IDictionary<Searchable, int> CountItemsInContainedShulkerBox(NbtCompound shulkerBox, ICollection<Searchable> searchedItems)
        {
            var results = new Dictionary<Searchable, int>();

            if (!shulkerBox.ContainsKey("components"))
            {
                return results;
            }

            var componentsTag = shulkerBox["components"] as NbtCompound;

            // Empty shulker boxes don't have a "minecraft:container".
            if (!componentsTag.ContainsKey("minecraft:container"))
            {
                return results;
            }

            // List of compound (slot,item)
            // where item is a compound (id, count)
            var containerContents = componentsTag["minecraft:container"] as NbtList;
            if (containerContents == null)
            {
                return results;
            }

            foreach (var slot in containerContents)
            {
                var slotTag = slot as NbtCompound;
                var itemTag = slotTag["item"] as NbtCompound;

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
        protected override bool IsMatchingPotion(NbtCompound itemTag, Potion searchedPotion)
        {
            var componentsTag = itemTag["components"] as NbtCompound;
            if (componentsTag == null)
            {
                return false;
            }

            var potionContentsTag = componentsTag["minecraft:potion_contents"] as NbtCompound;
            if (potionContentsTag == null)
            {
                return false;
            }

            var potionTag = potionContentsTag["potion"] as NbtString;
            if (potionTag == null)
            {
                return false;
            }

            return potionTag.Value == searchedPotion.PotionContents;
        }

        /// <summary>
        /// Indicates whether the provided item tag is an enchanted book matching the search.
        /// </summary>
        /// <param name="itemTag"></param>
        /// <param name="searchedBook"></param>
        /// <returns></returns>
        protected override bool IsMatchingEnchantedBook(NbtCompound itemTag, EnchantedBook searchedBook)
        {
            var componentsTag = itemTag["components"] as NbtCompound;
            if (componentsTag == null)
            {
                return false;
            }

            var storedEnchantmentsTag = componentsTag["minecraft:stored_enchantments"] as NbtCompound;
            if (storedEnchantmentsTag == null)
            {
                return false;
            }

            var levelsTag = storedEnchantmentsTag["levels"] as NbtCompound;
            if (levelsTag == null || !levelsTag.ContainsKey(searchedBook.Enchantment))
            {
                return false;
            }

            return (levelsTag[searchedBook.Enchantment] as NbtInt).Value == searchedBook.Level;
        }
    }
}