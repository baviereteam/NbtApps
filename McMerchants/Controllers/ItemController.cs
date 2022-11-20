using McMerchants.Models;
using Microsoft.AspNetCore.Mvc;

namespace McMerchants.Controllers
{
    public class ItemController : Controller
    {
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
                ItemId = id,
            });
        }
    }
}
