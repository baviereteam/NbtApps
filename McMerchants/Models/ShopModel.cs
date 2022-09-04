using NbtTools.Entities;
using NbtTools.Geography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace McMerchants.Models
{
    public class ShopModel
    {
        public Cuboid Shop { get; set; }

        public IDictionary<string, ICollection<Villager>> Villagers { get; set; }
    }
}
