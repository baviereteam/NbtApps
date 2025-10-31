using McMerchants.Models.Database;
using NbtTools.Entities.Trading;
using NbtTools.Geography;

namespace McMerchantsLib.Stock
{
    public class StockQueryResult
    {
        public IList<StoreStockResult> Stores { get; set; }
        public IDictionary<FactoryRegion, IDictionary<Point, int>> Factories { get; set; }

        public IDictionary<TradingRegion, IEnumerable<Trade>> Trades {  get; set; }

        public Boolean IsComplete { get; set; }

        public StockQueryResult()
        {
            Stores = new List<StoreStockResult>();
            Factories = new Dictionary<FactoryRegion, IDictionary<Point, int>>();
            Trades = new Dictionary<TradingRegion, IEnumerable<Trade>>();
            IsComplete = true;
        }
    }
}
