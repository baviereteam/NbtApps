using System.Text.Json.Serialization;

namespace NbtTools.Items
{
    public abstract class Searchable
    {
        [JsonPropertyName("value")]
        public string Id { get; private set; }

        [JsonPropertyName("label")]
        public string Name { get; private set; }

        public byte StackSize { get; private set; }

        protected Searchable(string id, string name, byte stackSize)
        {
            Id = id;
            Name = name;
            StackSize = stackSize;
        }
    }
}
