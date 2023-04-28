using McMerchants.Database;
using McMerchants.Json;
using McMerchants.Models.Database;
using McMerchants.Models.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using NbtTools.Geography;
using NbtTools.Items;
using NbtTools.Mca;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace McMerchants.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockApiController : ControllerBase
    {
        private readonly StoredItemService StoredItemService;
        private readonly IConfiguration Configuration;
        private readonly McMerchantsDbContext Context;

        public StockApiController(IConfiguration configuration, StoredItemService storedItemService, McMerchantsDbContext context)
        {
            Configuration = configuration;
            StoredItemService = storedItemService;
            Context = context;
        }

        // GET api/<StockController>/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            var results = new StockApiResult();

            IEnumerable<StorageRegion> stores = Context.StorageRegions.Include(s => s.Alleys);
            foreach (var store in stores)
            {
                results.Stores.Add(
                    FormatStoreResults(store, StoredItemService.FindStoredItems(id, store.Coordinates))
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

        private StoreStockResult FormatStoreResults(StorageRegion store, IDictionary<Point, int> searchResults)
        {
            var storeStock = new StoreStockResult(store);

            foreach (var result in searchResults)
            {
                if (result.Value > 0)
                {
                    storeStock.Count += result.Value;
                    try
                    {
                        storeStock.AlleysContaining.Add(store.Alleys.First(alley => IsPointInAlley(result.Key, alley)));
                    }
                    catch (Exception)
                    {
                        // Store has no alleys, or no alley matches this point
                    }
                }
            }

            storeStock.AlleysContaining = storeStock.AlleysContaining.Distinct().ToList();
            return storeStock;
        }

        private bool IsPointInAlley(Point p, Alley a)
        {
            Console.WriteLine($"Looking for point {p} in alley {a.Name}");
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
