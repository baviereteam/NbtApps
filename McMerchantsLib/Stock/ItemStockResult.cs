using NbtTools.Items;

namespace McMerchantsLib.Stock
{
    /// <summary>
    /// Search results for a single item type, combining stores, factories and trading places.
    /// </summary>
    public class ItemStockResult
    {
        public ICollection<StoreItemStockResult> Stores { get; set; }
        public ICollection<FactoryItemStockResult> Factories { get; set; }
        public ICollection<TradeItemStockResult> Trades { get; set; }

        public ItemStockResult()
        {
            Stores = new List<StoreItemStockResult>();
            Factories = new List<FactoryItemStockResult>();
            Trades = new List<TradeItemStockResult>();
        }
    }
}
