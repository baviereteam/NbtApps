using NbtTools.Entities.Trading;
using NbtTools.Geography;
using System.Collections.Generic;

namespace NbtTools.Entities
{
    public class Villager
    {
        public ICollection<Trade> Trades { get; set; }

        public string Job { get; private set; }
        public int Level { get; private set; }
        public Point Position { get; private set; }
        public string Type { get; private set; }

        public Villager(string job, int level, string type, Point position)
        {
            Job = job;
            Level = level;
            Type = type;
            Position = position;
        }
    }
}
