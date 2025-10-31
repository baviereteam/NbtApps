using McMerchants.Database;
using McMerchants.Models.Database;
using Microsoft.EntityFrameworkCore;
using NbtTools.Database;
using NbtTools.Entities;
using NbtTools.Geography;
using NbtTools.Items;

namespace McMerchantsLib.Stock
{
    public class StockService
    {
        private readonly StoredItemService StoredItemService;
        private readonly VillagerService VillagerService;
        private readonly McMerchantsDbContext Context;
        private readonly NbtDbContext NbtContext;

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

        public StockQueryResult GetStockOf(string itemId)
        {
            var searchedItem = NbtContext.Searchables
                .Include(searchable => (searchable as Potion).Type)
                .Single(searchable => searchable.Id == itemId);

            var results = new StockQueryResult();

            IEnumerable<StorageRegion> stores = Context.StorageRegions.Include(s => s.Alleys);
            foreach (var store in stores)
            {
                var storeQuery = StoredItemService.FindStoredItems(searchedItem, store.Coordinates);
                results.Stores.Add(SortIntoAlleys(store, itemId, storeQuery.Result));
                results.IsComplete &= storeQuery.IsComplete;
            }

            IEnumerable<FactoryRegion> factories = Context.FactoryProducts.Where(p => p.Item == itemId).Select(p => p.Factory);
            foreach (var factory in factories)
            {
                var factoryQuery = StoredItemService.FindStoredItems(searchedItem, factory.Coordinates);
                results.Factories.Add(factory, factoryQuery.Result);
                results.IsComplete &= factoryQuery.IsComplete;
            }

            var tradingPlaces = Context.TradingRegions;
            foreach (var tradingPlace in tradingPlaces)
            {
                var tradingQuery = VillagerService.GetTradesFor(tradingPlace.Coordinates, itemId);
                results.Trades.Add(tradingPlace, tradingQuery.Result);
                results.IsComplete &= tradingQuery.IsComplete;
            }

            return results;
        }

        private StoreStockResult SortIntoAlleys(StorageRegion store, string item, IDictionary<Point, int> searchResults)
        {
            var storeStock = new StoreStockResult(store);

            foreach (var result in searchResults)
            {
                if (result.Value > 0)
                {
                    try
                    {
                        // throws InvalidOperationException if there's no matches
                        Alley alley = store.Alleys.First(alley => IsPointInAlley(result.Key, alley));

                        if (storeStock.StockInOtherAlleys.ContainsKey(alley))
                        {
                            // There was already some stuff in this alley
                            storeStock.StockInOtherAlleys[alley] += result.Value;
                        }
                        else
                        {
                            // First time we hear about this alley
                            storeStock.StockInOtherAlleys.Add(
                                alley,
                                result.Value
                            );
                        }
                    }

                    catch (InvalidOperationException)
                    {
                        // Store has no alleys, or no alley matches this point
                        storeStock.StockInBulkContainers.Add(result);
                    }
                }
            }

            // Move the default alley stuff to the correct spot
            var defaultAlley = GetDefaultAlleyForItem(store, item);
            if (defaultAlley != null)
            {
                try
                {
                    var defaultAlleyResults = storeStock.StockInOtherAlleys.First(alleyEntry => alleyEntry.Key == defaultAlley);
                    storeStock.StockInDefaultAlley = new Tuple<Alley, int>(defaultAlleyResults.Key, defaultAlleyResults.Value);
                    storeStock.StockInOtherAlleys.Remove(defaultAlleyResults);
                }
                catch (InvalidOperationException)
                {
                    // There's nothing in the default alley
                    storeStock.StockInDefaultAlley = new Tuple<Alley, int>(defaultAlley, 0);
                }
            }

            return storeStock;
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

        private bool IsPointInAlley(Point p, Alley a)
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
    }
}
