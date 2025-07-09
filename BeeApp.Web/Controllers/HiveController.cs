using BeeApp.Shared.Data;
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

        public async Task<IActionResult> Index(int apiaryNumber)
        {
            var hives = await _context.Hives
                .Include(h => h.Apiary)
                .Include(h => h.Measurements)
                .Where(h => h.ApiaryId == apiaryNumber)
                .ToListAsync();

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

            return View(vm);
        }
    }
}
