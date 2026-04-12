using McMerchants.Database;
using McMerchants.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace McMerchants.Controllers
{
    public class StoreController : Controller
    {
        private readonly McMerchantsDbContext Context;

        public StoreController(McMerchantsDbContext context)
        {
            Context = context;
        }

        [Route("Stores")]
        public ActionResult List()
        {
            ICollection<StorageRegion> zones = Context.StorageRegions.ToList();
            return View(zones);
        }
    }
}
