namespace NbtTools.Entities.Trading
{
    public class Enchantment
    {
        public string Id { get; private set; }
        public short Level {  get; private set; }

        public Enchantment(string id, short level)
        {
            Id = id;
            Level = level;
        }

        public override string ToString()
        {
            return $"{Id} {Level}";
        }
    }
}
