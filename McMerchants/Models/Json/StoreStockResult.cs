using McMerchants.Models.Database;
using System.Collections.Generic;

namespace McMerchants.Models.Json
{
    public class StoreStockResult
    {
        public StorageRegion Store { get; set; }
        public int Count { get; set; }
        public ICollection<Alley> AlleysContaining { get; set; }

        public StoreStockResult(StorageRegion store) {
            Store = store;
            Count = 0;
            AlleysContaining = new List<Alley>();
        }
    }
}
