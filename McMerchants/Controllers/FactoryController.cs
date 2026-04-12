using McMerchants.Database;
using McMerchants.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace McMerchants.Controllers
{
    public class FactoryController : Controller
    {
        private readonly McMerchantsDbContext Context;

        public FactoryController(McMerchantsDbContext context)
        {
            Context = context;
        }

        [Route("Factories")]
        public ActionResult List()
        {
            ICollection<FactoryRegion> zones = Context.FactoryRegions.Include(f => f.Products).ToList();
            return View(zones);
        }
    }
}
