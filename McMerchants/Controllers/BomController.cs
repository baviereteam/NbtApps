using McMerchants.Database;
using McMerchants.Models;
using McMerchantsLib.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace McMerchants.Controllers
{
    [Route("Boms")]
    public class BomController : Controller
    {
        private readonly McMerchantsDbContext _context;
        private readonly IConfiguration _configuration;

        public BomController(McMerchantsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult List()
        {
            ICollection<Bom> boms = _context.Boms.Include(bom => bom.Items).ToList();
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
            Bom bom = _context.Boms.Single(bom => bom.Id == id);
            return View(new BomDetailsViewModel()
            {
                Bom = bom,
                ShowWorkzoneTutorial = _configuration?["Options:ShowBomWorkzoneTutorial"] == true.ToString()
            });
        }
    }
}
