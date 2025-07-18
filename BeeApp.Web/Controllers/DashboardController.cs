using BeeApp.Shared.Data;
using BeeApp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string range = "24h", bool? smoothing = null)
        {
            bool isSmoothing = smoothing ?? true;

            DateTime from = range switch
            {
                "4h" => DateTime.Now.AddHours(-4),
                "8h" => DateTime.Now.AddHours(-8),
                "12h" => DateTime.Now.AddHours(-12),
                "24h" => DateTime.Now.AddHours(-24),
                _ => DateTime.Now.AddHours(-24)
            };

            int aggregationHours = range switch
            {
                "24h" => 2,
                "12h" => 1,
                "8h" => 1,
                "4h" => 0, // žádná agregace
                _ => 2
            };

            var hives = await _context.Hives
                .Include(h => h.Apiary)
                .Include(h => h.Measurements.Where(m => m.MeasurementDate >= from))
                .ToListAsync();

            var viewModel = hives
                .Where(h => h.Measurements.Any())
                .Select(h => new HiveMiniChartViewModel
                {
                    HiveId = h.HiveId,
                    HiveName = h.Name,
                    ApiaryName = h.Apiary?.Name ?? "(unknown)",
                    DataPoints = aggregationHours == 0
                        ? h.Measurements.OrderBy(m => m.MeasurementDate)
                            .Select(m => new HiveMiniPoint
                            {
                                Timestamp = m.MeasurementDate,
                                Temperature = m.Temperature,
                                Weight = m.Weight
                            }).ToList()
                        : h.Measurements
                            .GroupBy(m => new DateTime(m.MeasurementDate.Year, m.MeasurementDate.Month, m.MeasurementDate.Day, m.MeasurementDate.Hour / aggregationHours * aggregationHours, 0, 0))
                            .Select(g => new HiveMiniPoint
                            {
                                Timestamp = g.Key,
                                Temperature = g.Average(x => x.Temperature),
                                Weight = g.Average(x => x.Weight)
                            })
                            .OrderBy(p => p.Timestamp)
                            .ToList(),

                    CurrentRange = range,
                    CurrentSmoothing = isSmoothing
                })
                .ToList();

            return View(viewModel);
        }
    }
}
