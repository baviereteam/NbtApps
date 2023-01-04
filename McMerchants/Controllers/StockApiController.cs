using McMerchants.Database;
using McMerchants.Json;
using McMerchants.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NbtTools.Geography;
using NbtTools.Items;
using NbtTools.Mca;
using System;
using System.Collections.Generic;
using System.Text.Json;

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
            IEnumerable<StorageRegion> stores = Context.StorageRegions;
            var results = new Dictionary<StorageRegion, IDictionary<Point, int>>();
            foreach (var store in stores)
            {
                results.Add(store, StoredItemService.FindStoredItems(id, store.Coordinates));
            }

            var json = ResultsToJson(results);
            return json;
        }

        private string ResultsToJson(IDictionary<StorageRegion, IDictionary<Point, int>> source)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new StoreItemCountConverter());
            return JsonSerializer.Serialize(source, options);
        }
    }
}
