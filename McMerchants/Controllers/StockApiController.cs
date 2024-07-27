using McMerchants.Json;
using McMerchantsLib.Stock;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace McMerchants.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockApiController : ControllerBase
    {
        private readonly StockApiResultConverter JsonConverter;
        private readonly StockService StockService;

        public StockApiController(StockApiResultConverter jsonConverter, StockService stockService)
        {
            JsonConverter = jsonConverter;
            StockService = stockService;
        }

        // GET api/stock/minecraft:glass
        [HttpGet("{id}")]
        public string Get(string id)
        {
            var results = StockService.GetStockOf(id);
            var json = ResultsToJson(results);
            return json;
        }        

        private string ResultsToJson(StockQueryResult result)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(JsonConverter);
            return JsonSerializer.Serialize(result, options);
        }
    }
}
