using McMerchants.Models.Database;
using NbtTools.Entities.Trading;

using StockAtPosition = System.Collections.Generic.KeyValuePair<NbtTools.Geography.Point, int>;

namespace McMerchantsLib.Stock
{
    public class StockQueryResult
    {
        public IList<StoreStockResult> Stores { get; set; }
        public IDictionary<FactoryRegion, ICollection<StockAtPosition>> Factories { get; set; }

        public IDictionary<TradingRegion, IEnumerable<Trade>> Trades {  get; set; }

        public Boolean IsComplete { get; set; }

        public StockQueryResult()
        {
            Stores = new List<StoreStockResult>();
            Factories = new Dictionary<FactoryRegion, ICollection<StockAtPosition>>();
            Trades = new Dictionary<TradingRegion, IEnumerable<Trade>>();
            IsComplete = true;
        }
    }
}
