using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiaryDataWeb.Controllers
{
    public class HiveController : Controller
    {
        private readonly AppDbContext _context;

        public HiveController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int apiaryId)
        {
            if (apiaryId == 0)
            {
                var hives = await _context.Hives.Include(h => h.Apiary).ToListAsync();
                ViewData["ApiaryId"] = 0;
                return View(hives);
            }
            else
            {
                var hives = await _context.Hives
                    .Where(h => h.ApiaryNumber == apiaryId)
                    .ToListAsync();
                ViewData["ApiaryId"] = apiaryId;
                return View(hives);
            }

            
            
        }

        public IActionResult Create(int apiaryId)
        {
            ViewBag.ApiaryId = apiaryId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Hive hive)
        {
            if (ModelState.IsValid)
            {
                _context.Hives.Add(hive);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { apiaryId = hive.ApiaryNumber });
            }
            return View(hive);
        }
    }
}
