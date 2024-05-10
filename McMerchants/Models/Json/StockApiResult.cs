using McMerchants.Models.Database;
using NbtTools.Entities.Trading;
using NbtTools.Geography;
using System.Collections.Generic;

namespace McMerchants.Models.Json
{
    public class StockApiResult
    {
        public IList<StoreStockResult> Stores { get; set; }
        public IDictionary<FactoryRegion, IDictionary<Point, int>> Factories { get; set; }

        public IDictionary<TradingRegion, IEnumerable<Trade>> Trades {  get; set; }

        public StockApiResult()
        {
            Stores = new List<StoreStockResult>();
            Factories = new Dictionary<FactoryRegion, IDictionary<Point, int>>();
            Trades = new Dictionary<TradingRegion, IEnumerable<Trade>>();
        }
    }
}
