using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiaryDataCollector.Models; // Namespace pro Hive, HiveMeasurement
using System.Linq;
using System.Threading.Tasks;
using ApiaryDataWeb.Models;

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
            var datasets = new List<DatasetForPlot>();

            foreach (var group in groupedMeasurements)
            {
                var weights = new double[10] { 1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9, 10.10 }; //group.Select(m => m.Weight).ToArray();
                var labels = new double[10] { 1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9, 10.10 };//group.Select(m => m.MeasurementDate.ToString("yyyy-MM-dd HH:mm")).ToArray();

                var dataset = new DatasetForPlot()
                {
                    label = $"Hive #{group.Key}",
                    data = weights,
                    fill = false,
                    borderColor = "#" + (new Random()).Next(0, 0xFFFFFF).ToString("X6"), // random color for each line
                    tension = 0.1
                };

                datasets.Add(dataset);
            }

            // Výpočet minima a maxima
            double globalMin = groupedMeasurements
                .SelectMany(group => group.Select(m => m.Weight))
                .Min()-1;

            double globalMax = groupedMeasurements
                .SelectMany(group => group.Select(m => m.Weight))
                .Max()+1;

            Random rand = new Random();
            // Testovací log pro kontrolu dat
            foreach (var dataset in datasets)
            {
                Console.WriteLine($"Label: {dataset.label}, Data: {string.Join(",", dataset.data)}, BorderColor: {dataset.borderColor}");
                
            }
            //test
            // Pass datasets to View
            ViewData["ChartDatasets"] = datasets;
            
            //ViewData["Labels"] = measurements.Select(m => m.MeasurementDate.ToString("yyyy-MM-dd HH:mm")).Distinct().ToArray();
            ViewData["GlobalMin"] = 0;
            ViewData["GlobalMax"] = 20;

            ViewData["SelectedHives"] = selectedHives;

            return View(measurements);
        }
    }
}