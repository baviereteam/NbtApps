using SharpNBT;

namespace NbtTools.Items.Providers
{
    // 1.21.10
    internal class Version4556StorageReader : Version3837StorageReader
    {
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
            if (storedEnchantmentsTag == null || !storedEnchantmentsTag.ContainsKey(searchedBook.Enchantment))
            {
                return false;
            }

            return (storedEnchantmentsTag[searchedBook.Enchantment] as IntTag).Value == searchedBook.Level;
        }
    }
}