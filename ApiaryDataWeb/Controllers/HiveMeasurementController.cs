using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiaryDataCollector.Models; // Namespace pro Hive, HiveMeasurement
using System.Linq;
using System.Threading.Tasks;
using ApiaryDataWeb.Models;
using Newtonsoft.Json;

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
        public async Task<IActionResult> Index(int? apiaryId, int[] selectedHives)
        {
            // Load data for dropdown filters
            ViewData["Apiaries"] = await _context.Apiaries.ToListAsync();
            ViewData["SelectedApiaryId"] = apiaryId;

            if (apiaryId.HasValue)
            {
                ViewData["Hives"] = await _context.Hives
                    .Where(h => h.ApiaryNumber == apiaryId)
                    .ToListAsync();
            }
            else
            {
                ViewData["Hives"] = Enumerable.Empty<Hive>();
            }

            // Pokud nejsou vybrány žádné úly, nebudeme načítat měření
            if (selectedHives == null || selectedHives.Length == 0)
            {
                ViewData["ChartDatasets"] = new List<DatasetForPlot>();
                ViewData["Labels"] = new string[0];
                ViewData["GlobalMin"] = 0;
                ViewData["GlobalMax"] = 0;
                return View(new List<HiveMeasurement>());
            }

            // Load measurements
            var measurementsQuery = _context.HiveMeasurement
                .Include(m => m.Hive)
                .ThenInclude(h => h.Apiary)
                .AsQueryable();

            if (selectedHives != null && selectedHives.Length > 0)
            {
                measurementsQuery = measurementsQuery.Where(m => selectedHives.Contains(m.HiveNumber));
            }
            else if (apiaryId.HasValue)
            {
                measurementsQuery = measurementsQuery.Where(m => m.Hive.ApiaryNumber == apiaryId);
            }

            var measurements = await measurementsQuery.ToListAsync();

            // Group measurements by HiveNumber
            var groupedMeasurements = measurements
                .GroupBy(m => m.HiveNumber)
                .ToList();

            // Create datasets for each hive
            //var datasets = new List<DatasetForPlot>();

            //foreach (var group in groupedMeasurements)
            //{

            //    var dataset = new DatasetForPlot()
            //    {
            //        label = $"Hive #{group.Key}",
            //        //data = weights,
            //        fill = false,
            //        borderColor = "#" + (new Random()).Next(0, 0xFFFFFF).ToString("X6"), // random color for each line
            //        tension = 0.1
            //    };

            //    datasets.Add(dataset);
            //}

            // Výpočet minima a maxima
            double globalMin = Math.Round(groupedMeasurements
                .SelectMany(group => group.Select(m => m.Weight))
                .Min()) - 5;

            double globalMax = Math.Round(groupedMeasurements
                .SelectMany(group => group.Select(m => m.Weight))
                .Max()) + 5;

            // Pass datasets to View
            /*var chartData = JsonConvert.SerializeObject(new
            {
                datasets = new[]
                {
                    new
                    {
                        label = "Teplota",
                        borderColor = "rgba(255, 99, 132, 1)", // Červená barva
                        hidden = false,
                        data = new[]
                        {
                            new { koko = "2025-01-01T10:00:00Z", value = 30 }, // koko is the time
                            new { koko = "2025-01-01T11:00:00Z", value = 21 },
                            new { koko = "2025-01-01T12:00:00Z", value = 20 },
                            new { koko = "2025-01-01T13:00:00Z", value = 12 },
                            new { koko = "2025-01-01T14:00:00Z", value = 7 }
                        }
                    },
                    new
                    {
                        label = "Vlhkost",
                        borderColor = "rgba(54, 162, 235, 1)", // Modrá barva
                        hidden = false,
                        data = new[]
                        {
                            new { koko = "2025-01-01T10:00:00Z", value = 30 },
                            new { koko = "2025-01-01T11:00:00Z", value = 25 },
                            new { koko = "2025-01-01T12:15:00Z", value = 20 },
                            new { koko = "2025-01-01T12:30:00Z", value = 35 },
                            new { koko = "2025-01-01T14:00:00Z", value = 37 }
                        }
                    }
                }
            });*/

            var datasets = new List<DatasetForPlot>();

            foreach (var group in groupedMeasurements)
            {

                // Naplnění dat ze skupiny
                var dataPoints = group.Select(m => new Data
                {
                    koko = m.MeasurementDate.ToString("yyyy-MM-ddTHH:mm:ssZ"), // ISO8601 formát času
                    value = (float)m.Weight // Zaokrouhlení Weight na celé číslo (Data.value je int)
                }).ToArray();


                // Vytvoření datasetu pro tuto skupinu
                var datasetForPlot = new DatasetForPlot
                {
                    label = $"Hive #{group.Key}",
                    borderColor = "#" + (new Random()).Next(0, 0xFFFFFF).ToString("X6"), // Náhodná barva
                    hidden = false,
                    data = dataPoints
                };

                datasets.Add(datasetForPlot);
            }
            //{

            //    var dataset = new DatasetForPlot()
            //    {
            //        label = $"Hive #{group.Key}",
            //        //data = weights,
            //        fill = false,
            //        borderColor = "#" + (new Random()).Next(0, 0xFFFFFF).ToString("X6"), // random color for each line
            //        tension = 0.1
            //    };

            //    datasets.Add(dataset);
            ViewBag.ChartData = JsonConvert.SerializeObject(new { datasets });
            //ViewData["Labels"] = measurements.Select(m => m.MeasurementDate.ToString("yyyy-MM-dd HH:mm")).Distinct().ToArray();

            ViewBag.TimeUnit = "hour";
            ViewBag.Minimum = globalMin;
            ViewBag.Maximum = globalMax;

            ViewData["SelectedHives"] = selectedHives;

            return View(measurements);
        }


        public async Task<IActionResult> Overview(int? apiaryId, int[] selectedHives)
        {
            // Load data for dropdown filters
            ViewData["Apiaries"] = await _context.Apiaries.ToListAsync();
            ViewData["SelectedApiaryId"] = apiaryId;

            if (apiaryId.HasValue)
            {
                ViewData["Hives"] = await _context.Hives
                    .Where(h => h.ApiaryNumber == apiaryId)
                    .ToListAsync();
            }
            else
            {
                ViewData["Hives"] = Enumerable.Empty<Hive>();
            }


            
             // Pokud nejsou vybrány žádné úly, nebudeme načítat měření
            if (selectedHives == null || selectedHives.Length == 0)
            {
                ViewData["ChartDatasets"] = new List<DatasetForPlot>();
                ViewData["Labels"] = new string[0];
                ViewData["GlobalMin"] = 0;
                ViewData["GlobalMax"] = 0;
                return View(new List<HiveMeasurement>());
            }

            // Load measurements
            var measurementsQuery = _context.HiveMeasurement
                .Include(m => m.Hive)
                .ThenInclude(h => h.Apiary)
                .AsQueryable();

            if (selectedHives != null && selectedHives.Length > 0)
            {
                measurementsQuery = measurementsQuery.Where(m => selectedHives.Contains(m.HiveNumber));
            }
            else if (apiaryId.HasValue)
            {
                measurementsQuery = measurementsQuery.Where(m => m.Hive.ApiaryNumber == apiaryId);
            }




            var measurements = await measurementsQuery.ToListAsync();

            // Group measurements by HiveNumber
            var groupedMeasurements = measurements
                .GroupBy(m => m.HiveNumber)
                .ToList();


            var hiveCharts = groupedMeasurements.Select(group =>
            {
                var hiveMeasurements = group.ToList();

                // Data pro váhu a teplotu
                var dataPoints = hiveMeasurements.Select(m => new
                {
                    Time = m.MeasurementDate.ToString("yyyy-MM-ddTHH:mm:ssZ"), // ISO8601
                    Weight = m.Weight,
                    Temperature = m.Temperature
                }).ToList();

                // Výpočet minima a maxima
                double weightMin = Math.Floor(hiveMeasurements.Min(m => m.Weight)) - 5;
                double weightMax = Math.Ceiling(hiveMeasurements.Max(m => m.Weight)) + 5;
                double tempMin = Math.Floor(hiveMeasurements.Min(m => m.Temperature)) - 5;
                double tempMax = Math.Ceiling(hiveMeasurements.Max(m => m.Temperature)) + 5;

                return new
                {
                    HiveNumber = group.Key,
                    Data = dataPoints,
                    WeightMin = weightMin,
                    WeightMax = weightMax,
                    TempMin = tempMin,
                    TempMax = tempMax
                };
            }).ToList();

            ViewBag.ChartData = JsonConvert.SerializeObject(hiveCharts);
            ViewData["SelectedHives"] = selectedHives;

            return View(measurements);
        }
    }
}