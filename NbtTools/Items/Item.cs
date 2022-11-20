using System.ComponentModel.DataAnnotations;

namespace NbtTools.Items
{
    public class Item
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public byte StackSize { get; private set; }

        public Item(string id, string name, byte stackSize)
        {
            Id = id;
            Name = name;
            StackSize = stackSize;
        }
    }
}
