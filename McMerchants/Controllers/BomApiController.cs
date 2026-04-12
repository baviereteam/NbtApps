using System.Linq;
using McMerchants.Json.Bom;
using McMerchantsLib.Bom;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using McMerchants.Database;
using McMerchants.Models.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using NbtTools.Geography;

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

        [HttpGet]
        [Route("{id:int}/available")]
        public string GetAvailability(int id, [FromQuery] WorkzoneCoordinatesDTO workzoneCoords)
        {
            var bom = _context.Boms
                .Include(bom => bom.Items)
                .Single(bom => bom.Id == id);

            Cuboid? workzone = GetWorkzoneFromQuery(workzoneCoords);
            var items = _bomService.GetAvailabilityOf(bom, workzone);
            return JsonSerializer.Serialize(items, _serializerOptions);
        }

        private static Cuboid? GetWorkzoneFromQuery(WorkzoneCoordinatesDTO dto)
        {
            if (
                dto.Dimension != null
                && dto.StartX.HasValue
                && dto.StartY.HasValue
                && dto.StartZ.HasValue
                && dto.EndX.HasValue
                && dto.EndY.HasValue
                && dto.EndZ.HasValue
            )
            {
                var start = new Point(dto.StartX.Value, dto.StartY.Value, dto.StartZ.Value);
                var end = new Point(dto.EndX.Value, dto.EndY.Value, dto.EndZ.Value);
                return new Cuboid(dto.Dimension, start, end);
            }
            else
            {
                return null;
            }
        }
    }
}
