using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NbtTools.Database;
using NbtTools.Items;

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

        // GET: api/items/cia%20bo
        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<Item>>> SearchItems(string search)
        {
            return await _context.Items.Where(item => item.Name.ToLower().Contains(search.ToLower())).ToListAsync();
        }
    }
}
