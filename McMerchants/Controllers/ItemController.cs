using McMerchants.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NbtTools.Database;
using System.Linq;

namespace McMerchants.Controllers
{
    public class ItemController : Controller
    {
        private readonly NbtDbContext _context;
        private readonly IConfiguration _configuration;

        public ItemController(NbtDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: ItemController
        [Route("")]
        [Route("Item")]
        [Route("Item/Details")]
        public ActionResult Index()
        {
            return View("Details", null);
        }

        // GET: item/Details/minecraft:spruce_sapling
        [Route("Item/Details/{itemId}")]
        public ActionResult Details(string itemId)
        {
            return View(new ItemViewModel
            {
                Item = _context.Searchables.First(item => item.Id == itemId),
                MapLinkTitle = _configuration["Options:WebmapLinkTitle"]?.ToString() ?? "Map",
                CustomLinkTitle = _configuration["Options:CustomLinkTitle"]?.ToString() ?? "Web"
            });
        }
    }
}
