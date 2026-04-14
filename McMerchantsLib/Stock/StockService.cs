using McMerchants.Database;
using McMerchants.Models.Database;
using Microsoft.EntityFrameworkCore;
using NbtTools.Database;
using NbtTools.Entities;
using NbtTools.Geography;
using NbtTools.Items;

using QuantitiesByPosition = System.Collections.Generic.IDictionary<NbtTools.Geography.Point, int>;
using Trades = System.Collections.Generic.ICollection<NbtTools.Entities.Trading.Trade>;

namespace McMerchantsLib.Stock
{
    public class StockService
    {
        private readonly StoredItemService StoredItemService;
        private readonly VillagerService VillagerService;
        private readonly McMerchantsDbContext Context;
        private readonly NbtDbContext NbtContext;
        private readonly IDictionary<Point, int> EmptyStoreResult = new Dictionary<Point, int>();

        public StockService(
            StoredItemService storedItemService,
            VillagerService villagerService,
            McMerchantsDbContext context,
            NbtDbContext nbtContext
        )
        {
            StoredItemService = storedItemService;
            VillagerService = villagerService;
            Context = context;
            NbtContext = nbtContext;
        }

        public StockSearchResults GetStockOf(params string[] ids)
        {
            // Must be a list (or something that implements Contains natively = not an array), or else :
            // System.TypeLoadException: GenericArguments[1], 'System.ReadOnlySpan`1[System.String]', on 'System.Linq.Expressions.Interpreter.FuncCallInstruction`2[T0,TRet]' violates the constraint of type parameter 'TRet'.
            var itemIds = ids.ToList(); 

            var searchedItems = NbtContext.Searchables
                .Include(searchable => (searchable as Potion).Type)
                .Where(searchable => itemIds.Contains(searchable.Id))
                .ToList();

            return GetStockOf(searchedItems);
        }

        public StockSearchResults GetStockOf(ICollection<Searchable> searchedItems)
        {
            var results = new StockSearchResults(searchedItems);

            IEnumerable<StorageRegion> stores = Context.StorageRegions.Include(s => s.Alleys);
            foreach (var store in stores)
            {
                // EACH STORE must be present in the response even if they don't have the item
                var storeQuery = StoredItemService.FindStoredItems(searchedItems, store.Coordinates);
                results.IsComplete &= storeQuery.IsComplete;
                results.InsertStoresForItems(
                    SplitStoreResultsByItem(store, searchedItems, storeQuery.Results)
                );
            }

            IEnumerable<FactoryRegion> factories = ListFactoriesProducingOneOf(searchedItems);
            foreach (var factory in factories)
            {
                var factoryQuery = StoredItemService.FindStoredItems(searchedItems, factory.Coordinates);
                results.IsComplete &= factoryQuery.IsComplete;
                results.InsertFactoriesForItems(
                    SplitFactoryResultsByItem(factory, factoryQuery.Results)
                );
            }

            var tradingPlaces = Context.TradingRegions;
            foreach (var tradingPlace in tradingPlaces)
            {
                var tradingQuery = VillagerService.GetTradesFor(tradingPlace.Coordinates, searchedItems);
                results.IsComplete &= tradingQuery.IsComplete;
                results.InsertTradingPlacesForItems(
                    SplitTradingResultsByItem(tradingPlace, tradingQuery.Results)
                );
            }

            return results;
        }

        private IDictionary<Searchable, StoreItemStockResult> SplitStoreResultsByItem(StorageRegion store, ICollection<Searchable> searchedItems,  IDictionary<Searchable, QuantitiesByPosition> searchResults)
        {
            var results = new Dictionary<Searchable, StoreItemStockResult>();

            foreach (var searchedItem in searchedItems)
            {
                // Stores that don't carry the item must still appear in the results
                var searchResultForItem = searchResults.ContainsKey(searchedItem) ? searchResults[searchedItem] : EmptyStoreResult;
                results.Add(searchedItem, SortIntoAlleys(store, searchedItem, searchResultForItem));
            }

            return results;
        }

        private StoreItemStockResult SortIntoAlleys(StorageRegion store, Searchable item, QuantitiesByPosition stockOfItem)
        {
            var sortedResult = new StoreItemStockResult(store);

            foreach (var result in stockOfItem)
            {
                if (result.Value > 0)
                {
                    try
                    {
                        // throws InvalidOperationException if there's no matches
                        Alley alley = store.Alleys.First(alley => IsPointInAlley(result.Key, alley));

                        if (sortedResult.StockInOtherAlleys.ContainsKey(alley))
                        {
                            // There was already some stuff in this alley
                            sortedResult.StockInOtherAlleys[alley] += result.Value;
                        }
                        else
                        {
                            // First time we hear about this alley
                            sortedResult.StockInOtherAlleys.Add(
                                alley,
                                result.Value
                            );
                        }
                    }

                    catch (InvalidOperationException)
                    {
                        // Store has no alleys, or no alley matches this point
                        sortedResult.StockInBulkContainers.Add(result);
                    }
                }
            }

            // Move the default alley stuff to the correct spot
            var defaultAlley = GetDefaultAlleyForItem(store, item.Id);
            if (defaultAlley != null)
            {
                try
                {
                    var defaultAlleyResults = sortedResult.StockInOtherAlleys.First(alleyEntry => alleyEntry.Key == defaultAlley);
                    sortedResult.StockInDefaultAlley = new Tuple<Alley, int>(defaultAlleyResults.Key, defaultAlleyResults.Value);
                    sortedResult.StockInOtherAlleys.Remove(defaultAlleyResults);
                }
                catch (InvalidOperationException)
                {
                    // There's nothing in the default alley
                    sortedResult.StockInDefaultAlley = new Tuple<Alley, int>(defaultAlley, 0);
                }
            }

            return sortedResult;
        }

        private Alley GetDefaultAlleyForItem(StorageRegion store, string id)
        {
            try
            {
                return Context.DefaultAlleys
                .Include(da => da.Alley)
                .Where(d => d.Item == id)
                .First(d => d.Alley.Store == store)
                .Alley;
            }

            // No element satisfies the condition in predicate.
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private static bool IsPointInAlley(Point p, Alley a)
        {
            if (p.Y < a.StartY || p.Y > a.EndY)
            {
                return false;
            }

            if (a.Direction == Alley.AlleyDirection.X && a.Coordinate == p.X && a.LowBoundary <= p.Z && p.Z <= a.HighBoundary)
            {
                return true;
            }

            if (a.Direction == Alley.AlleyDirection.Z && a.Coordinate == p.Z && a.LowBoundary <= p.X && p.X <= a.HighBoundary)
            {
                return true;
            }

            return false;
        }

        private IEnumerable<FactoryRegion> ListFactoriesProducingOneOf(ICollection<Searchable> searchedItems)
        {
            var searchedItemIds = searchedItems.Select(item => item.Id);
            return Context.FactoryProducts
                .Where(p => searchedItemIds.Contains(p.Item))
                .Select(p => p.Factory);
        }
        private static IDictionary<Searchable, FactoryItemStockResult> SplitFactoryResultsByItem(FactoryRegion factory, IDictionary<Searchable, QuantitiesByPosition> searchResults)
        {
            var results = new Dictionary<Searchable, FactoryItemStockResult>();

            foreach (var itemResult in searchResults)
            {
                results.Add(
                    itemResult.Key, 
                    new FactoryItemStockResult
                    {
                        Factory = factory,
                        Stock = itemResult.Value
                    }
                );
            }

            return results;
        }
        private static IDictionary<Searchable, TradeItemStockResult> SplitTradingResultsByItem(TradingRegion tradingPlace, IDictionary<Searchable, Trades> searchResults)
        {
            var results = new Dictionary<Searchable, TradeItemStockResult>();

            foreach (var itemResult in searchResults)
            {
                results.Add(
                    itemResult.Key,
                    new TradeItemStockResult
                    {
                        TradingPlace = tradingPlace,
                        Trades = itemResult.Value
                    }
                );
            }

            return results;
        }
    }
}
