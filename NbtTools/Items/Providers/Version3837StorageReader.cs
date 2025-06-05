using SharpNBT;

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
        internal override int CountItemsInContainedShulkerBox(CompoundTag shulkerBox, Searchable searchedItem)
        {
            // Removed "tag" and replaced with "components" which is a key-value map.
            var componentsTag = shulkerBox["components"] as CompoundTag;

            // Empty shulker boxes don't have a "minecraft:container".
            if (componentsTag == null || !componentsTag.ContainsKey("minecraft:container"))
            {
                return 0;
            }

            // List of compound (slot,item)
            // where item is a compound (id, count)
            var containerContents = componentsTag["minecraft:container"] as ListTag;
            if (containerContents == null)
            {
                return 0;
            }

            int count = 0;
            foreach (var slot in containerContents)
            {
                var slotTag = slot as CompoundTag;
                var itemTag = slotTag["item"] as CompoundTag;
                count += GetCountFromItemSlot(itemTag, searchedItem);
            }

            return count;
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
    }
}
