using McMerchants.Database;
using McMerchants.Models;
using McMerchants.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace McMerchants.Controllers
{
    [Route("Stores")]
    public class StoreController : Controller
    {
        private readonly McMerchantsDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public StoreController(McMerchantsDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View(await _context.StorageRegions.ToListAsync());
        }

        // GET: Store/5/Edit
        [HttpGet("{id:int}/Edit")]
        [Authorize(Policy = Program.POLICY_IS_IN_DISCORD_SERVER)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.StorageRegions.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            ViewData["AvailableDimensions"] = GetAvailableDimensions();
            ViewData["AvailableLogos"] = GetAvailableLogos();

            return View(store);
        }

        // POST: Store/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("{id:int}/Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Program.POLICY_IS_IN_DISCORD_SERVER)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Logo,Dimension,URL,StartX,StartY,StartZ,EndX,EndY,EndZ")] StorageRegion store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }

            ViewData["AvailableDimensions"] = GetAvailableDimensions();
            ViewData["AvailableLogos"] = GetAvailableLogos();

            return View(store);
        }

        private bool StoreExists(int id)
        {
            return _context.StorageRegions.Any(e => e.Id == id);
        }

        private ICollection<SelectListItem> GetAvailableDimensions()
        {
            var mapEntries = _configuration.GetSection("MapPaths").GetChildren();
            return mapEntries
                .Select(entry => new SelectListItem() 
                {
                    Value = entry.Key,
                    Text = entry.Key
                })
                .ToList();
        }

        private ICollection<SelectListItem> GetAvailableLogos()
        {
            var imagesPath = Path.Combine(_environment.WebRootPath, "img/stores");
            return Directory
                .EnumerateFiles(imagesPath, "*.png", SearchOption.TopDirectoryOnly)
                .Select(path =>
                {
                    var fileName = Path.GetFileName(path);
                    return new SelectListItem()
                    {
                        Value = fileName,
                        Text = fileName
                    };
                })
                .ToList();
        }
    }
}
