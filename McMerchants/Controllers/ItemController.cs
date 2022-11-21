using McMerchants.Models;
using Microsoft.AspNetCore.Mvc;
using NbtTools.Database;
using System.Linq;

namespace McMerchants.Controllers
{
    public class ItemController : Controller
    {
        private readonly NbtDbContext _context;

        public ItemController(NbtDbContext context)
        {
            _context = context;
        }

        // GET: ItemController
        public ActionResult Index()
        {
            return View("Details", null);
        }

        // GET: item/Details/minecraft:spruce_sapling
        public ActionResult Details(string? id)
        {
            if (id == null)
            {
                Index();
            }

            return View(new ItemModel
            {
                Item = _context.Items.First(item => item.Id == id),
            });
        }
    }
}
