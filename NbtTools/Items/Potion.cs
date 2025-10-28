namespace NbtTools.Items
{
    public class Potion : Searchable
    {
        public Item Type { get; set; }
        public string PotionContents { get; set; }

        public Potion(string id, string name, byte stackSize) : base(id, name, stackSize)
        {
        }

        public Potion(string id, string name, Item type, string contents) : base(id, name, type.StackSize)
        {
            Type = type;
            PotionContents = contents;
        }
    }
}
