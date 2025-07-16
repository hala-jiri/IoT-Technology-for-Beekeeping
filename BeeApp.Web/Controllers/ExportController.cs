using BeeApp.Shared.Data;
using BeeApp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text;

namespace BeeApp.Web.Controllers
{
    public class ExportController : Controller
    {
        private readonly AppDbContext _context;

        public ExportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new ExportDataRequestViewModel
            {
                AvailableApiaries = await _context.Apiaries.Include(a => a.Hives).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportCsv(ExportDataRequestViewModel request)
        {
            var from = request.From ?? DateTime.MinValue;
            var to = request.To ?? DateTime.MaxValue;

            var selectedHives = await _context.Hives
                .Include(h => h.Measurements)
                .Include(h => h.InspectionReports)
                .Where(h => request.SelectedHiveIds.Contains(h.HiveId))
                .ToListAsync();

            var measurementsLines = new List<string> { "Hive,Date,Weight,Temperature" };
            var inspectionsLines = new List<string> { "Hive,Date,QueenSeen,BroodPresent,EggsPresent,PollenPresent,HoneyPresent,Notes" };

            foreach (var hive in selectedHives)
            {
                if (request.IncludeMeasurements)
                {
                    foreach (var m in hive.Measurements
                        .Where(m => m.MeasurementDate >= from && m.MeasurementDate <= to)
                        .OrderBy(m => m.MeasurementDate))
                    {
                        measurementsLines.Add($"{hive.Name},{m.MeasurementDate:s},{m.Weight},{m.Temperature}");
                    }
                }

                if (request.IncludeInspections)
                {
                    foreach (var i in hive.InspectionReports
                        .Where(i => i.InspectionDate >= from && i.InspectionDate <= to)
                        .OrderBy(i => i.InspectionDate))
                    {
                        inspectionsLines.Add($"{hive.Name},{i.InspectionDate:s},{i.QueenSeen},{i.BroodPresent},{i.EggsPresent},{i.PollenPresent},{i.HoneyPresent},\"{i.Notes?.Replace("\"", "\"\"")}\"");
                    }
                }
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                if (request.IncludeMeasurements)
                {
                    var entry = archive.CreateEntry("measurements.csv");
                    using var writer = new StreamWriter(entry.Open());
                    foreach (var line in measurementsLines)
                        writer.WriteLine(line);
                }

                if (request.IncludeInspections)
                {
                    var entry = archive.CreateEntry("inspections.csv");
                    using var writer = new StreamWriter(entry.Open());
                    foreach (var line in inspectionsLines)
                        writer.WriteLine(line);
                }
            }

            var zipBytes = memoryStream.ToArray();
            var fileName = $"export-{DateTime.Now:yyyyMMddHHmmss}.zip";
            return File(zipBytes, "application/zip", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ExportJson(ExportDataRequestViewModel request)
        {
            var from = request.From ?? DateTime.MinValue;
            var to = request.To ?? DateTime.MaxValue;

            var selectedHives = await _context.Hives
                .Include(h => h.Measurements)
                .Include(h => h.InspectionReports)
                .Where(h => request.SelectedHiveIds.Contains(h.HiveId))
                .ToListAsync();

            var exportObject = new
            {
                From = from,
                To = to,
                ExportedAt = DateTime.Now,
                Hives = selectedHives.Select(h => new
                {
                    HiveId = h.HiveId,
                    HiveName = h.Name,
                    Measurements = request.IncludeMeasurements
                        ? h.Measurements
                            .Where(m => m.MeasurementDate >= from && m.MeasurementDate <= to)
                            .OrderBy(m => m.MeasurementDate)
                            .Select(m => new
                            {
                                m.MeasurementDate,
                                m.Weight,
                                m.Temperature
                            })
                        : null,
                    Inspections = request.IncludeInspections
                        ? h.InspectionReports
                            .Where(i => i.InspectionDate >= from && i.InspectionDate <= to)
                            .OrderBy(i => i.InspectionDate)
                            .Select(i => new
                            {
                                i.InspectionDate,
                                i.QueenSeen,
                                i.BroodPresent,
                                i.EggsPresent,
                                i.PollenPresent,
                                i.HoneyPresent,
                                i.Notes
                            })
                        : null
                })
            };

            var json = System.Text.Json.JsonSerializer.Serialize(exportObject, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var fileName = $"export-{DateTime.Now:yyyyMMddHHmmss}.json";
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            return File(jsonBytes, "application/json", fileName);
        }
    }
}
