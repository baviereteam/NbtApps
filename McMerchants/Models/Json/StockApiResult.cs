using McMerchants.Models.Database;
using NbtTools.Geography;
using System.Collections.Generic;

namespace McMerchants.Models.Json
{
    public class StockApiResult
    {
        public IDictionary<ItemProviderRegion, IDictionary<Point, int>> Stores { get; set; }
        public IDictionary<ItemProviderRegion, IDictionary<Point, int>> Factories { get; set; }

        public StockApiResult()
        {
            Stores = new Dictionary<ItemProviderRegion, IDictionary<Point, int>>();
            Factories = new Dictionary<ItemProviderRegion, IDictionary<Point, int>>();
        }
    }
}
