using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using BeeApp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

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
            ViewBag.Lat = apiary?.Latitude;
            ViewBag.Lng = apiary?.Longitude;
            //TODO: Map doesnt load properly. Data are loaded properly, but JS didnt process it properly

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

        public async Task<IActionResult> Detail(int id, string range = "14d", int? aggregationHours = null, bool? smoothing = null)
        {
            bool isSmoothing = smoothing ?? true;

            var hive = await _context.Hives
                .Include(h => h.Apiary)
                .Include(h => h.Measurements)
                .FirstOrDefaultAsync(h => h.HiveId == id);

            if (hive == null) return NotFound();


            var now = DateTime.Now;
            DateTime from = range switch
            {
                "1D" => now.AddHours(-now.Hour),
                "24h" => now.AddHours(-24),
                "7d" => now.AddDays(-7),
                "14d" => now.AddDays(-14),
                _ => now.AddDays(-14)
            };

            int agg = aggregationHours ?? range switch
            {
                "24h" => 0,
                "7d" => 2,
                "14d" => 8,
                _ => 4
            };

            var rawData = hive.Measurements
                .Where(m => m.MeasurementDate >= from && m.MeasurementDate <= now)
                .OrderBy(m => m.MeasurementDate)
                .ToList();

            List<HiveMeasurementPoint> chartData;

            if (agg == 0)
            {
                // žádná agregace
                chartData = rawData
                    .Select(m => new HiveMeasurementPoint
                    {
                        Date = m.MeasurementDate,
                        Weight = Math.Round(m.Weight, 2),
                        Temperature = Math.Round(m.Temperature, 2)
                    })
                    .ToList();
            }
            else
            {
                chartData = rawData
                    .GroupBy(m => new DateTime(
                        m.MeasurementDate.Year,
                        m.MeasurementDate.Month,
                        m.MeasurementDate.Day,
                        (m.MeasurementDate.Hour / agg) * agg, 0, 0)) // agregace po X hodinách
                    .Select(g => new HiveMeasurementPoint
                    {
                        Date = g.Key,
                        Weight = Math.Round(g.Average(x => x.Weight), 2),
                        Temperature = Math.Round(g.Average(x => x.Temperature), 2)
                    })
                    .ToList();
            }

            var lastHiveMeasurement = rawData.LastOrDefault();
            var lastApiaryMeasurement = await _context.ApiaryMeasurements
                .Where(a => a.ApiaryId == hive.ApiaryId)
                .OrderByDescending(m => m.MeasurementDate)
                .FirstOrDefaultAsync();


            var lastInspection = await _context.InspectionReports
                .Where(r => r.HiveId == id)
                .OrderByDescending(r => r.InspectionDate)
                .FirstOrDefaultAsync();

            var viewModel = new HiveDetailViewModel
            {
                HiveId = hive.HiveId,
                HiveName = hive.Name,
                ApiaryName = hive.Apiary?.Name,
                LastHiveMeasurementDate = lastHiveMeasurement?.MeasurementDate,
                LastWeight = lastHiveMeasurement?.Weight,
                LastHiveTemp = lastHiveMeasurement?.Temperature,
                LastApiaryMeasurementDate = lastApiaryMeasurement?.MeasurementDate,
                LastApiaryTemp = lastApiaryMeasurement?.Temperature,
                LastApiaryLight = lastApiaryMeasurement?.LightIntensity,
                LastInspection = lastInspection,
                MeasurementsForChart = rawData,
                ChartData = chartData,
                CurrentRange = range,
                CurrentAggregation = agg,
                CurrentSmoothing = isSmoothing
            };


            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Export(int id)
        {
            var hive = await _context.Hives
                .Include(h => h.Measurements)
                .FirstOrDefaultAsync(h => h.HiveId == id);

            if (hive == null || hive.Measurements.Count == 0)
                return NotFound("No measurements to export.");

            var csvLines = new List<string>
            {
                "HiveId,Date,Weight,Temperature"
            };

            var fromDate = DateTime.Now.AddDays(-90);

            foreach (var m in hive.Measurements.Where(m => m.MeasurementDate >= fromDate).OrderBy(m => m.MeasurementDate))
            {
                csvLines.Add($"{m.HiveId},{m.MeasurementDate:s},{m.Weight},{m.Temperature}");
            }

            var fileName = $"hive-{id}-export-{DateTime.Now:yyyyMMddHHmmss}.csv";
            var fileBytes = Encoding.UTF8.GetBytes(string.Join("\n", csvLines));
            return File(fileBytes, "text/csv", fileName);
        }

    }
}
