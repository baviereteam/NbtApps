using System.Text.Json.Serialization;

namespace NbtTools.Items
{
    public class Item
    {
        [JsonPropertyName("value")]
        public string Id { get; private set; }

        [JsonPropertyName("label")]
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
