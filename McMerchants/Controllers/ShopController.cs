using McMerchants.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NbtTools.Entities;
using NbtTools.Geography;
using NbtTools.Mca;

namespace McMerchants.Controllers
{
    public class ShopController : Controller
    {
        private VillagerService villagerService = new VillagerService();
        private readonly IConfiguration Configuration;

        public ShopController(IConfiguration conf)
        {
            Configuration = conf;
        }

        // GET: TradeController/Details/5
        public ActionResult Details(int fromX, int fromY, int fromZ, int toX, int toY, int toZ)
        {
            McaFile.RootPath = Configuration["MapPaths:Entities"];

            var startPoint = new Point(fromX, fromY, fromZ);
            var endPoint = new Point(toX, toY, toZ);
            var shopZone = new Cuboid(startPoint, endPoint);

            var villagers = villagerService.OrderByJob(villagerService.GetVillagers(shopZone));

            return View(new ShopModel { 
                Shop = shopZone,
                Villagers = villagers
            });
        }
    }
}
