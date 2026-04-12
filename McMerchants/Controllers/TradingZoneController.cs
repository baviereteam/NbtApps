using McMerchants.Database;
using McMerchants.Models;
using McMerchants.Models.Database;
using Microsoft.AspNetCore.Mvc;
using NbtTools.Entities;
using System.Collections.Generic;
using System.Linq;

namespace McMerchants.Controllers
{
    public class TradingZoneController : Controller
    {
        private readonly VillagerService VillagerService;
        private readonly McMerchantsDbContext Context;

        public TradingZoneController(VillagerService villagerService, McMerchantsDbContext context)
        {
            VillagerService = villagerService;
            Context = context;
        }

        [Route("Trading")]
        public ActionResult List()
        {
            ICollection<TradingRegion> zones = Context.TradingRegions.ToList();
            return View(zones);
        }

        [Route("Trading/Details/{zoneId}")]
        public ActionResult Details(int zoneId)
        {
            TradingRegion zone = Context.TradingRegions.Find(zoneId);
            if (zone == null)
            {
                return View("NotFound");
            }

            var villagersQuery = VillagerService.GetVillagers(zone.Coordinates);
            var sortedVillagers = VillagerService.OrderByJob(villagersQuery.Result);

            return View(new TradingZoneViewModel { 
                Zone = zone,
                Villagers = sortedVillagers,
                IsComplete = villagersQuery.IsComplete
            });
        }
    }
}
