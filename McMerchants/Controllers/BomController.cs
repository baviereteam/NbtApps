using McMerchants.Database;
using McMerchantsLib.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace McMerchants.Controllers
{
    [Route("Boms")]
    public class BomController : Controller
    {
        private readonly McMerchantsDbContext Context;

        public BomController(McMerchantsDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public IActionResult List()
        {
            ICollection<Bom> boms = Context.Boms.Include(bom => bom.Items).ToList();
            return View(boms);
        }

        [HttpGet]
        [Route("new")]
        public IActionResult New()
        {
            return View();
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult Details(int id) 
        {
            Bom bom = Context.Boms.Single(bom => bom.Id == id);
            return View(bom);
        }
    }
}
