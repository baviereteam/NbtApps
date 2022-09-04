using NbtTools.Geography;
using NbtTools.Nbt;
using SharpNBT;
using System;
using System.Collections.Generic;

namespace NbtTools.Entities
{
    public class VillagerService
    {
        private NbtFilter nbtFilter = new NbtFilter();
        private RegionQuery regionQuery = new RegionQuery();

        public ICollection<Villager> GetVillagers(Cuboid zone)
        {
            var dataSource = regionQuery.GetEntitiesDataSource(zone);
            var villagerTags = nbtFilter.GetAllCompoundsWithId(dataSource, "minecraft:villager");
            var villagers = new List<Villager>();

            foreach (var villagerTag in villagerTags)
            {
                var villager = VillagerFactory.FromNbtTag(villagerTag);
                if (villager.Position.ContainedIn(zone))
                {
                    villagers.Add(villager);
                }
            }

            return villagers;
        }

        public IDictionary<string, ICollection<Villager>> OrderByJob(ICollection<Villager> source)
        {
            var destination = new Dictionary<string, ICollection<Villager>>();

            foreach (var villager in source)
            {
                if (!destination.ContainsKey(villager.Job))
                {
                    destination[villager.Job] = new List<Villager>();
                }

                destination[villager.Job].Add(villager);
            }

            return destination;
        }
    }
}
