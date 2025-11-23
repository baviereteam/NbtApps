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
        private readonly PluginApiConverter PluginApiConverter;
        private readonly WebApiConverter WebApiConverter;
        private readonly StockService StockService;

        public StockApiController(PluginApiConverter pluginApiConverter, WebApiConverter webApiConverter, StockService stockService)
        {
            PluginApiConverter = pluginApiConverter;
            WebApiConverter = webApiConverter;
            StockService = stockService;
        }

        // GET api/stock/minecraft:glass
        [HttpGet("{id}")]
        public string Get(string id, [FromQuery] bool synthetic = false)
        {
            var results = StockService.GetStockOf(id);
            var json = ResultsToJson(results, synthetic);
            return json;
        }

        private string ResultsToJson(StockQueryResult result, bool usePluginView)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(usePluginView ? PluginApiConverter : WebApiConverter);
            return JsonSerializer.Serialize(result, options);
        }
    }
}
