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
        [Route("Shop/Details/{shopId}")]
        public ActionResult Details(int shopId)
        {
            TradingRegion shop = Context.TradingRegions.Find(shopId);
            if (shop == null)
            {
                return View("NotFound");
            }

            var villagersQuery = VillagerService.GetVillagers(shop.Coordinates);
            var sortedVillagers = VillagerService.OrderByJob(villagersQuery.Result);

            return View(new ShopViewModel { 
                Shop = shop,
                Villagers = sortedVillagers,
                IsComplete = villagersQuery.IsComplete  //TODO: make CSS for this
            });
        }
    }
}
