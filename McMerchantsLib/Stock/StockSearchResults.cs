using NbtTools.Items;

namespace McMerchantsLib.Stock
{
    public class StockSearchResults
    {
        public IDictionary<Searchable, ItemStockResult> Results { get; }
        public bool IsComplete { get; internal set; }

        public StockSearchResults(ICollection<Searchable> searchedItems)
        {
            Results = new Dictionary<Searchable, ItemStockResult>();

            foreach (var searchedItem in searchedItems)
            {
                Results.Add(searchedItem, new ItemStockResult());
            }
        }

        public void InsertStoresForItems(IDictionary<Searchable, StoreItemStockResult> itemsInStores)
        {
            foreach (var entry in itemsInStores)
            {
                Results[entry.Key].Stores.Add(entry.Value);
            }
        }

        public void InsertFactoriesForItems(IDictionary<Searchable, FactoryItemStockResult> itemsInFactories)
        {
            foreach (var entry in itemsInFactories)
            {
                Results[entry.Key].Factories.Add(entry.Value);
            }
        }

        public void InsertTradingPlacesForItems(IDictionary<Searchable, TradeItemStockResult> itemsInTradingPlaces)
        {
            foreach(var entry in itemsInTradingPlaces)
            {
                Results[entry.Key].Trades.Add(entry.Value);
            }
        }
    }
}
