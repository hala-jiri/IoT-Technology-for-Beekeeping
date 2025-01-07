using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiaryDataCollector.Models; // Namespace pro Hive, HiveMeasurement
using System.Linq;
using System.Threading.Tasks;

namespace ApiaryDataWeb.Controllers
{
    public class HiveMeasurementController : Controller
    {
        private readonly AppDbContext _context;

        public HiveMeasurementController(AppDbContext context)
        {
            _context = context;
        }

        // GET: HiveMeasurement/Index
        public async Task<IActionResult> Index(int? apiaryId, int? hiveId)
        {
            // Pro načtení dat do dropdown filtrů
            ViewData["Apiaries"] = await _context.Apiaries.ToListAsync();
            ViewData["SelectedApiaryId"] = apiaryId;

            if (apiaryId.HasValue)
            {
                ViewData["Hives"] = await _context.Hives
                    .Where(h => h.ApiaryNumber == apiaryId)
                    .ToListAsync();
                ViewData["SelectedHiveId"] = hiveId;
            }
            else
            {
                ViewData["Hives"] = Enumerable.Empty<Hive>();
                ViewData["SelectedHiveId"] = null;
            }

            // Načtení měření
            var measurements = _context.HiveMeasurement
                .Include(m => m.Hive)
                .ThenInclude(h => h.Apiary)
                .AsQueryable();

            if (hiveId.HasValue)
            {
                measurements = measurements.Where(m => m.HiveNumber == hiveId);
            }
            else if (apiaryId.HasValue)
            {
                measurements = measurements.Where(m => m.Hive.ApiaryNumber == apiaryId);
            }

            return View(await measurements.ToListAsync());
        }
    }
}