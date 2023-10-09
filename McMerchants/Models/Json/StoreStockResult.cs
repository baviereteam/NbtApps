using McMerchants.Models.Database;
using NbtTools.Geography;
using System;
using System.Collections.Generic;

namespace McMerchants.Models.Json
{
    public class StoreStockResult
    {
        public StorageRegion Store { get; set; }

        public Tuple<Alley, int> StockInDefaultAlley { get; set; }
        public IDictionary<Alley, int> StockInOtherAlleys { get; set; }
        public IDictionary<Point, int> StockInBulkContainers { get; set; }

        public StoreStockResult(StorageRegion store) {
            Store = store;
            StockInDefaultAlley = null;
            StockInOtherAlleys = new Dictionary<Alley, int>();
            StockInBulkContainers = new Dictionary<Point, int>();
        }
    }
}
