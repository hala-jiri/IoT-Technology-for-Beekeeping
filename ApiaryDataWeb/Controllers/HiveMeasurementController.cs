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
        public async Task<IActionResult> Index(int? apiaryId, int[] selectedHives, string selectedDateRange = "Last24h")
        {
            // Load data for dropdown filters
            ViewData["Apiaries"] = await _context.Apiaries.ToListAsync();
            ViewData["SelectedApiaryId"] = apiaryId;
            //ViewBag.DateRange = dateRange; // Uložení vybraného rozsahu do ViewBag


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
                ViewData["SelectedDateRange"] = selectedDateRange ?? "Last24h"; // Defaultně Last24h
                return View(new List<HiveMeasurement>());
            }

            ViewData["SelectedHives"] = selectedHives;
            ViewData["SelectedDateRange"] = selectedDateRange ?? "Last24h"; // Defaultně Last24h

            if (selectedHives == null || selectedHives.Length == 0)
            {
                ViewBag.ChartData = JsonConvert.SerializeObject(new { datasets = new List<DatasetForPlot>() });
                return View(new List<HiveMeasurement>());
            }

            // Load measurements
            var measurementsQuery = _context.HiveMeasurement
                .Include(m => m.Hive)
                .ThenInclude(h => h.Apiary)
                .Where(m => selectedHives.Contains(m.HiveNumber));

            /*if (selectedHives != null && selectedHives.Length > 0)
            {
                measurementsQuery = measurementsQuery.Where(m => selectedHives.Contains(m.HiveNumber));
            }
            else if (apiaryId.HasValue)
            {
                measurementsQuery = measurementsQuery.Where(m => m.Hive.ApiaryNumber == apiaryId);
            }*/

            // Aplikace filtru na základě časového rozsahu
            var now = DateTime.UtcNow;
            switch (selectedDateRange)
            {
                case "Last24h":
                    measurementsQuery = measurementsQuery.Where(m => m.MeasurementDate >= now.AddDays(-1) && m.MeasurementDate <= now);
                    break;
                case "Last7Days":
                    measurementsQuery = measurementsQuery.Where(m => m.MeasurementDate >= now.AddDays(-7) && m.MeasurementDate <= now);
                    break;
                case "Last30Days":
                    measurementsQuery = measurementsQuery.Where(m => m.MeasurementDate >= now.AddDays(-30) && m.MeasurementDate <= now);
                    break;
                case "All":
                default:
                    // Neaplikovat žádný filtr
                    break;
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
            double globalMin = 0; //default value
            double globalMax = 80; //default value
            if (groupedMeasurements.Any())
            {
                globalMin = Math.Round(groupedMeasurements
                .SelectMany(group => group.Select(m => m.Weight))
                .Min()) - 5;

                globalMax = Math.Round(groupedMeasurements
                    .SelectMany(group => group.Select(m => m.Weight))
                    .Max()) + 5;
            }

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
            
            ViewBag.ChartData = JsonConvert.SerializeObject(new { datasets });
            //ViewData["Labels"] = measurements.Select(m => m.MeasurementDate.ToString("yyyy-MM-dd HH:mm")).Distinct().ToArray();

            ViewBag.TimeUnit = "hour";
            ViewBag.Minimum = globalMin;
            ViewBag.Maximum = globalMax;



            return View(measurements);
        }


        public async Task<IActionResult> Overview(int? apiaryId, int[] selectedHives, string selectedDateRange = "Last24h")
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
                ViewData["SelectedDateRange"] = selectedDateRange ?? "Last24h"; // Defaultně Last24h
                return View(new List<HiveMeasurement>());
            }

            ViewData["SelectedHives"] = selectedHives;
            ViewData["SelectedDateRange"] = selectedDateRange ?? "Last24h"; // Defaultně Last24h


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

            // Aplikace filtru na základě časového rozsahu
            var now = DateTime.UtcNow;
            switch (selectedDateRange)
            {
                case "Last24h":
                    measurementsQuery = measurementsQuery.Where(m => m.MeasurementDate >= now.AddDays(-1) && m.MeasurementDate <= now);
                    break;
                case "Last7Days":
                    measurementsQuery = measurementsQuery.Where(m => m.MeasurementDate >= now.AddDays(-7) && m.MeasurementDate <= now);
                    break;
                case "Last30Days":
                    measurementsQuery = measurementsQuery.Where(m => m.MeasurementDate >= now.AddDays(-30) && m.MeasurementDate <= now);
                    break;
                case "All":
                default:
                    // Neaplikovat žádný filtr
                    break;
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

            return View(measurements);
        }
    }
}