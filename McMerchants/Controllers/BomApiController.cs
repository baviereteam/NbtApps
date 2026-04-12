using System.Linq;
using McMerchants.Json.Bom;
using McMerchantsLib.Bom;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using McMerchants.Database;
using McMerchants.Models.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;

namespace McMerchants.Controllers
{
    [Route("api/bom")]
    [ApiController]
    public class BomApiController : ControllerBase
    {
        private readonly BomService _bomService;
        private readonly McMerchantsDbContext _context;
        private readonly JsonSerializerOptions _serializerOptions;

        public BomApiController(
	        BomService bomService,
	        BomImportResultConverter importResultConverter,
	        BomItemConverter itemConverter,
	        McMerchantsDbContext context)
        {
            _bomService = bomService;
            _context = context;
            
            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(importResultConverter);
            _serializerOptions.Converters.Add(itemConverter);
        }

        [HttpPost]
        [Route("create")]
        [Consumes("application/json")]
        public string Create([FromBody] BomImportRequestDTO requestDto)
        {
            var result = _bomService.ImportBom(requestDto.Name, requestDto.Data);
            return JsonSerializer.Serialize(result, _serializerOptions);
        }

        [HttpGet]
        [Route("{id:int}/items")]
        public string GetBomItems(int id)
        {
	        var bom = _context.Boms
		        .Include(bom => bom.Items)
		        .Single(bom => bom.Id == id);

	        var items = _bomService.GetItemsOf(bom);
	        return JsonSerializer.Serialize(items, _serializerOptions);
        }
    }
}
