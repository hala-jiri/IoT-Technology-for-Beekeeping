using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using BeeApp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Controllers
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
            var hives = await _context.Hives
                .Include(h => h.Apiary)
                .Include(h => h.Measurements)
                .Where(h => h.ApiaryId == apiaryId)
                .ToListAsync();

            // Get name of apiary
            var apiary = await _context.Apiaries.FindAsync(apiaryId);

            var vm = hives.Select(h => new HiveViewModel
            {
                HiveId = h.HiveId,
                Name = h.Name,
                ApiaryId = h.ApiaryId,
                ApiaryName = h.Apiary.Name,
                LastMeasurement = h.Measurements.OrderByDescending(m => m.MeasurementDate).FirstOrDefault()?.MeasurementDate,
                LastWeight = h.Measurements.OrderByDescending(m => m.MeasurementDate).FirstOrDefault()?.Weight,
                LastTemperature = h.Measurements.OrderByDescending(m => m.MeasurementDate).FirstOrDefault()?.Temperature
            }).ToList();

            // Add apiary info to the empty hive list
            if (!vm.Any() && apiary != null)
            {
                vm.Add(new HiveViewModel
                {
                    ApiaryId = apiary.ApiaryId,
                    ApiaryName = apiary.Name
                });
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create(int apiaryId)
        {
            var dto = new HiveCreateDto
            {
                ApiaryId = apiaryId
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HiveCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var hive = new Hive
            {
                Name = dto.Name,
                ApiaryId = dto.ApiaryId
            };

            _context.Hives.Add(hive);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { apiaryId = dto.ApiaryId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hive = await _context.Hives.FindAsync(id);
            if (hive == null) return NotFound();

            var dto = new HiveUpdateDto
            {
                HiveId = hive.HiveId,
                Name = hive.Name,
                ApiaryId = hive.ApiaryId
            };

            ViewBag.Apiaries = await _context.Apiaries.ToListAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HiveUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Apiaries = await _context.Apiaries.ToListAsync();
                return View(dto);
            }

            var hive = await _context.Hives.FindAsync(dto.HiveId);
            if (hive == null) return NotFound();

            hive.Name = dto.Name;
            hive.ApiaryId = dto.ApiaryId;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { apiaryId = dto.ApiaryId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var hive = await _context.Hives
                .Include(h => h.Apiary)
                .FirstOrDefaultAsync(h => h.HiveId == id);

            if (hive == null)
                return NotFound();

            return View(hive);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int HiveId)
        {
            var hive = await _context.Hives.FindAsync(HiveId);
            if (hive == null)
                return NotFound();

            int apiaryId = hive.ApiaryId;

            _context.Hives.Remove(hive);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { apiaryId });
        }
    }
}
