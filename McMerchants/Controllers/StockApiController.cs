using McMerchants.Database;
using McMerchants.Json;
using McMerchants.Models.Database;
using McMerchants.Models.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NbtTools.Geography;
using NbtTools.Items;
using NbtTools.Mca;
using System;
using System.Collections.Generic;
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

            IEnumerable<StorageRegion> stores = Context.StorageRegions;
            foreach (var store in stores)
            {
                results.Stores.Add(store, StoredItemService.FindStoredItems(id, store.Coordinates));
            }

            IEnumerable<FactoryRegion> factories = Context.FactoryProducts.Where(p => p.Item == id).Select(p => p.Factory);
            foreach (var factory in factories)
            {
                results.Factories.Add(factory, StoredItemService.FindStoredItems(id, factory.Coordinates));
            }

            var json = ResultsToJson(results);
            return json;
        }

        private string ResultsToJson(StockApiResult result)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new StockApiResultConverter());
            return JsonSerializer.Serialize(result, options);
        }
    }
}
