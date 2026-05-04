using McMerchants.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace McMerchants.Controllers
{
    [Route("Stores")]
    public class StoreController : Controller
    {
        private readonly McMerchantsDbContext _context;

        public StoreController(McMerchantsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View(await _context.StorageRegions.ToListAsync());
        }
    }
}
