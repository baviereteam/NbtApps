using fNbt;
using System.Collections.Generic;
using System.Linq;

namespace NbtTools.Items.Providers
{
    public class StorageReader
    {
        protected virtual int GetCountFromItemTag(NbtCompound itemTag)
        {
            return (itemTag["Count"] as NbtByte).Value;
        }

        /// <summary>
        /// Indicates whether an item ID designates a shulker box, regardless of its color.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual bool IsShulkerBox(string id)
        {
            return id.Contains("shulker_box");
        }

        /// <summary>
        /// Indicates whether the provided item tag is a potion matching the search.
        /// </summary>
        /// <param name="itemTag"></param>
        /// <param name="searchedPotion"></param>
        /// <returns></returns>
        protected virtual bool IsMatchingPotion(NbtCompound itemTag, Potion searchedPotion)
        {
            return false;   // search for potions is not supported before 1.20.5.
        }

        /// <summary>
        /// Indicates whether the provided item tag is an enchanted book matching the search.
        /// </summary>
        /// <param name="itemTag"></param>
        /// <param name="searchedBook"></param>
        /// <returns></returns>
        protected virtual bool IsMatchingEnchantedBook(NbtCompound itemTag, EnchantedBook searchedBook)
        {
            return false;   // search for enchanted books is not supported before 1.20.5.
        }

        /// <summary>
        /// Count the number of matching items in a container block (block containing an Items list tag).
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="searchedItem"></param>
        /// <returns></returns>
        internal virtual IDictionary<Searchable, int> CountMatchingItemsInContainer(NbtCompound storage, ICollection<Searchable> searchedItems)
        {
            var results = new Dictionary<Searchable, int>();

            if (!storage.ContainsKey("Items"))
            {
                return results;
            }

            var itemsTag = storage["Items"] as NbtList;

            // each non-empty slot in the container
            foreach (NbtTag t in itemsTag)
            {
                var tag = t as NbtCompound;
                var itemIdTag = tag["id"] as NbtString;

                // the slot might contain a shulkerbox
                if (IsShulkerBox(itemIdTag.Value))
                {
                    var shulkerBoxContentsMatching = CountItemsInContainedShulkerBox(tag, searchedItems);
                    results.AddRange(shulkerBoxContentsMatching);
                }

                // or an amount of a GIVEN item,
                var searchableThatMatchesThisItem = searchedItems.SingleOrDefault(searchable => ItemTagIs(tag, searchable), null);
                if (searchableThatMatchesThisItem == null)
                {
                    continue;
                }

                // There might already be some of that item in the container, already counted.
                results.AddOrIncrement(searchableThatMatchesThisItem, GetCountFromItemTag(tag));
            }

            return results;
        }

        /// <summary>
        /// Defines whether an item tag matches a given searched element.
        /// </summary>
        /// <param name="itemTag"></param>
        /// <param name="searchedItem"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        internal virtual bool ItemTagIs(NbtCompound itemTag, Searchable searchedItem)
        {
            var itemIdTag = itemTag["id"] as NbtString;
            return searchedItem switch 
            {
                Item _ => itemIdTag.Value == searchedItem.Id,
                EnchantedBook book => itemIdTag.Value == EnchantedBook.GENERIC_ENCHANTED_BOOK_ID && IsMatchingEnchantedBook(itemTag, book),
                Potion potion => itemIdTag.Value == potion.Type.Id && IsMatchingPotion(itemTag, potion),
                _ => throw new System.ArgumentException("Searchable of unknown type")
            };
        }

        /// <summary>
        /// Counts the matching items in a shulker box item (in another container).
        /// For a shulker box block, use <c>CountItemsIn</c>.
        /// </summary>
        /// <param name="shulkerBox"></param>
        /// <param name="searchedItems"></param>
        /// <returns></returns>
        internal virtual IDictionary<Searchable, int> CountItemsInContainedShulkerBox(NbtCompound shulkerBox, ICollection<Searchable> searchedItems)
        {
            var results = new Dictionary<Searchable, int>();

            var tagTag = shulkerBox["tag"] as NbtCompound;
            if (tagTag == null)
            {
                return results;
            }

            var blockEntityTag = tagTag["BlockEntityTag"] as NbtCompound;
            if (blockEntityTag == null)
            {
                return results;
            }

            return CountMatchingItemsInContainer(blockEntityTag, searchedItems);
        }

        internal virtual ICollection<string> ListItemsIn(NbtCompound storage)
        {
            List<string> results = new List<string>();

            var itemsTag = storage["Items"] as NbtList;
            if (itemsTag == null)
            {
                return results;
            }

            // each non-empty slot in the container
            foreach (NbtTag t in itemsTag)
            {
                var itemTag = t as NbtCompound;
                var itemIdTag = itemTag["id"] as NbtString;

                if (IsShulkerBox(itemIdTag.Value))
                {
                    results.AddRange(ListItemsInShulkerBox(itemTag));
                }
                else
                {
                    results.Add(itemIdTag.Value);
                }
            }

            return results;
        }

        protected virtual ICollection<string> ListItemsInShulkerBox(NbtCompound shulkerBox)
        {
            List<string> results = new List<string>();

            var tagTag = shulkerBox["tag"] as NbtCompound;
            if (tagTag == null)
            {
                return results;
            }

            var blockEntityTag = tagTag["BlockEntityTag"] as NbtCompound;
            if (blockEntityTag == null)
            {
                return results;
            }

            return ListItemsIn(blockEntityTag);
        }
    }
}
