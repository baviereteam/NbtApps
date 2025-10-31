using System;

namespace NbtTools.Entities.Trading
{
    public class Enchantment
    {
        public string Id { get; private set; }
        public int Level {  get; private set; }

        public Enchantment(string id, int level)
        {
            Id = id;
            Level = level;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Enchantment)obj;
            return Id == other.Id && Level == other.Level;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Level);
        }

        public override string ToString()
        {
            return $"{Id} {Level}";
        }
    }
}
