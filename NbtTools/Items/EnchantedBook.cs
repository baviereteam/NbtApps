namespace NbtTools.Items
{
    public class EnchantedBook : Searchable
    {
        public const string GENERIC_ENCHANTED_BOOK_ID = "minecraft:enchanted_book";

        public string Enchantment {  get; set; }
        public int Level { get; set; }

        public EnchantedBook(string id, string name, byte stackSize) : base(id, name, stackSize)
        {
        }

        public EnchantedBook(string id, string name, Item enchantedBookItem, string enchantment, int level) : base(id, name, enchantedBookItem.StackSize)
        {
            Enchantment = enchantment;
            Level = level;
        }
    }
}
