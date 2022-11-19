using McMerchants.Json;
using McMerchants.Models;
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

        public StockApiController(IConfiguration configuration)
        {
            Configuration = configuration;
            StoredItemService = new StoredItemService();
        }

        // GET api/<StockController>/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            McaFile.RootPath = Configuration["MapPaths:Regions"];
            var results = StoredItemService.FindItemInAllStores(id, TemporaryStoreList.GetStores());
            var json = ResultsToJson(results);
            return json;
        }

        private string ResultsToJson(IDictionary<string, IDictionary<Point, int>> source)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new StoreItemCountConverter());
            return JsonSerializer.Serialize(source, options);
        }
    }
}
