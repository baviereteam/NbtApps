using McMerchants.Models.Database;
using NbtTools.Entities;
using System.Collections.Generic;

namespace McMerchants.Models
{
    public class ShopViewModel
    {
        public TradingRegion Shop { get; set; }

        public IDictionary<string, ICollection<Villager>> Villagers { get; set; }
        public bool IsComplete { get; set; }
    }
}
