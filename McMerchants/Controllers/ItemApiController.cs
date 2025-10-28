using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NbtTools.Database;
using NbtTools.Items;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace McMerchants.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemApiController : ControllerBase
    {
        private readonly NbtDbContext _context;

        public ItemApiController(NbtDbContext context)
        {
            _context = context;
        }

        // GET: api/items?term=cia%20bo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Searchable>>> SearchItems([FromQuery] string term)
        {
            return await _context.Searchables.Where(item => item.Name.ToLower().Contains(term.ToLower())).ToListAsync();
        }
    }
}
