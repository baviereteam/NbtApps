using McMerchants.Json.Stock;
using McMerchants.Models.DTO;
using McMerchantsLib.Stock;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
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
        [EnableCors("GET from allowed origins")]
        public string Get(string id, [FromQuery] bool synthetic = false)
        {
            var results = StockService.GetStockOf(id);
            if (results.Results.Count != 1)
            {
                throw new InvalidDataException("There were more than one result for a single item search.");
            }

            var dto = new SingleItemStockResultDTO()
            {
                SearchResult = results.Results.First().Value,
                IsComplete = results.IsComplete
            };
            var json = ResultsToJson(dto, synthetic);
            return json;
        }

        private string ResultsToJson(SingleItemStockResultDTO result, bool usePluginView)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(usePluginView ? PluginApiConverter : WebApiConverter);
            return JsonSerializer.Serialize(result, options);
        }
    }
}
