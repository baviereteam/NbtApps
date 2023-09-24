using McMerchants.Database;
using McMerchants.Json;
using McMerchants.Models.Database;
using McMerchants.Models.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NbtTools.Geography;
using NbtTools.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace McMerchants.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockApiController : ControllerBase
    {
        private readonly StoredItemService StoredItemService;
        private readonly McMerchantsDbContext Context;

        public StockApiController(StoredItemService storedItemService, McMerchantsDbContext context)
        {
            StoredItemService = storedItemService;
            Context = context;
        }

        // GET api/stock/minecraft:glass
        [HttpGet("{id}")]
        public string Get(string id)
        {
            var results = new StockApiResult();

            IEnumerable<StorageRegion> stores = Context.StorageRegions.Include(s => s.Alleys);
            foreach (var store in stores)
            {
                results.Stores.Add(
                    FormatStoreResults(store, id, StoredItemService.FindStoredItems(id, store.Coordinates))
                );
            }

            IEnumerable<FactoryRegion> factories = Context.FactoryProducts.Where(p => p.Item == id).Select(p => p.Factory);
            foreach (var factory in factories)
            {
                results.Factories.Add(factory, StoredItemService.FindStoredItems(id, factory.Coordinates));
            }

            var json = ResultsToJson(results);
            return json;
        }

        private StoreStockResult FormatStoreResults(StorageRegion store, string item, IDictionary<Point, int> searchResults)
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
            if (defaultAlley != null) {
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

        private string ResultsToJson(StockApiResult result)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new StockApiResultConverter());
            return JsonSerializer.Serialize(result, options);
        }
    }
}
