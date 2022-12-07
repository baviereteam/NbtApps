using McMerchants.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NbtTools.Entities;
using NbtTools.Geography;
using NbtTools.Mca;
using System;

namespace McMerchants.Controllers
{
    public class ShopController : Controller
    {
        private VillagerService VillagerService;
        private readonly IConfiguration Configuration;

        public ShopController(IConfiguration conf, VillagerService villagerService)
        {
            Configuration = conf;
            VillagerService = villagerService;
        }

        // GET: TradeController/Details/5
        public ActionResult Details(int fromX, int fromY, int fromZ, int toX, int toY, int toZ)
        {
            //TODO: Rewrite this controller to only use a "shop zone" ID and pull the coordinates (including dimension) from a database
            var startPoint = new Point(fromX, fromY, fromZ);
            var endPoint = new Point(toX, toY, toZ);
            //TODO: once shop zone informations are in a database, use the actual dimension from the data to allow shop zones in other worlds.
            var shopZone = new Cuboid("overworld", startPoint, endPoint);

            if (shopZone.Size > 1000000)
            {
                throw new Exception("Zone too large.");
            }

            var villagers = VillagerService.OrderByJob(VillagerService.GetVillagers(shopZone));

            return View(new ShopViewModel { 
                Shop = shopZone,
                Villagers = villagers
            });
        }
    }
}
