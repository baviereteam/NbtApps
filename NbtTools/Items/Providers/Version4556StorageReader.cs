using fNbt;

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
        protected override bool IsMatchingEnchantedBook(NbtCompound itemTag, EnchantedBook searchedBook)
        {
            var componentsTag = itemTag["components"] as NbtCompound;
            if (componentsTag == null)
            {
                return false;
            }

            var storedEnchantmentsTag = componentsTag["minecraft:stored_enchantments"] as NbtCompound;
            if (storedEnchantmentsTag == null || !storedEnchantmentsTag.ContainsKey(searchedBook.Enchantment))
            {
                return false;
            }

            return (storedEnchantmentsTag[searchedBook.Enchantment] as NbtInt).Value == searchedBook.Level;
        }
    }
}