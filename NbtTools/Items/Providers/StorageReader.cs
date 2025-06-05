using SharpNBT;
using System.Collections.Generic;

namespace NbtTools.Items.Providers
{
    public class StorageReader
    {
        protected virtual int GetCountFromItemTag(CompoundTag itemTag)
        {
            return (itemTag["Count"] as ByteTag).Value;
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
        protected virtual bool IsMatchingPotion(CompoundTag itemTag, Potion searchedPotion)
        {
            return false;   // search for potions is not supported before 1.20.5.
        }

        /// <summary>
        /// Count the number of matching items in a container block (block containing an Items list tag).
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="searchedItem"></param>
        /// <returns></returns>
        internal virtual int CountItemsIn(CompoundTag storage, Searchable searchedItem)
        {
            var itemsTag = storage["Items"] as ListTag;
            if (itemsTag == null)
            {
                return 0;
            }

            int count = 0;

            // each non-empty slot in the container
            foreach (Tag t in itemsTag)
            {
                count += GetCountFromItemSlot(t as CompoundTag, searchedItem);
            }

            return count;
        }

        /// <summary>
        /// Count the number of matching items in an "item" compound (id, count, components).
        /// </summary>
        /// <param name="itemTag">A CompoundTag containing an id tag and optionally a count and a component tags.</param>
        /// <param name="searchedItem"></param>
        /// <returns></returns>
        internal virtual int GetCountFromItemSlot(CompoundTag itemTag, Searchable searchedItem)
        {
            var itemIdTag = itemTag["id"] as StringTag;

            if (IsShulkerBox(itemIdTag))
            {
                return CountItemsInContainedShulkerBox(itemTag, searchedItem);
            }

            switch (searchedItem)
            {
                case Item _:
                    if (itemIdTag.Value == searchedItem.Id)
                    {
                        return GetCountFromItemTag(itemTag);
                    }
                    else
                    {
                        return 0;
                    }

                case Potion potion:
                    if (itemIdTag.Value == potion.Type.Id && IsMatchingPotion(itemTag, potion))
                    {
                        return GetCountFromItemTag(itemTag);
                    }
                    else
                    {
                        return 0;
                    }

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Counts the matching items in a shulker box item (in another container).
        /// For a shulker box block, use <c>CountItemsIn</c>.
        /// </summary>
        /// <param name="shulkerBox"></param>
        /// <param name="searchedItem"></param>
        /// <returns></returns>
        internal virtual int CountItemsInContainedShulkerBox(CompoundTag shulkerBox, Searchable searchedItem)
        {
            var tagTag = shulkerBox["tag"] as CompoundTag;
            if (tagTag == null)
            {
                return 0;
            }

            var blockEntityTag = tagTag["BlockEntityTag"] as CompoundTag;
            if (blockEntityTag == null)
            {
                return 0;
            }

            return CountItemsIn(blockEntityTag, searchedItem);
        }

        internal virtual ICollection<string> ListItemsIn(CompoundTag storage)
        {
            List<string> results = new List<string>();

            var itemsTag = storage["Items"] as ListTag;
            if (itemsTag == null)
            {
                return results;
            }

            // each non-empty slot in the container
            foreach (Tag t in itemsTag)
            {
                var itemTag = t as CompoundTag;
                var itemIdTag = itemTag["id"] as StringTag;

                if (IsShulkerBox(itemIdTag))
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

        protected virtual ICollection<string> ListItemsInShulkerBox(CompoundTag shulkerBox)
        {
            List<string> results = new List<string>();

            var tagTag = shulkerBox["tag"] as CompoundTag;
            if (tagTag == null)
            {
                return results;
            }

            var blockEntityTag = tagTag["BlockEntityTag"] as CompoundTag;
            if (blockEntityTag == null)
            {
                return results;
            }

            return ListItemsIn(blockEntityTag);
        }
    }
}
