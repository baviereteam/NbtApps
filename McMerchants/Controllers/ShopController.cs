using McMerchants.Database;
using McMerchants.Models;
using McMerchants.Models.Database;
using Microsoft.AspNetCore.Mvc;
using NbtTools.Entities;
using NbtTools.Geography;
using System;

namespace McMerchants.Controllers
{
    public class ShopController : Controller
    {
        private readonly VillagerService VillagerService;
        private readonly McMerchantsDbContext Context;

        public ShopController(VillagerService villagerService, McMerchantsDbContext context)
        {
            VillagerService = villagerService;
            Context = context;
        }

        // GET: TradeController/Details/5
        public ActionResult Details(int shopId)
        {
            TradingRegion shop = Context.TradingRegions.Find(shopId);
            if (shop == null)
            {
                return View("NotFound");
            }

            var villagers = VillagerService.OrderByJob(VillagerService.GetVillagers(shop.Coordinates));

            return View(new ShopViewModel { 
                Shop = shop,
                Villagers = villagers
            });
        }
    }
}
