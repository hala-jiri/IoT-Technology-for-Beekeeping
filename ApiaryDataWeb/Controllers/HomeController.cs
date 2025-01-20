using ApiaryDataCollector.Models;
using ApiaryDataWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ApiaryDataWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Start časovače
            var stopwatch = Stopwatch.StartNew();

            var homeOverviewViewModel = new HomeOverviewViewModel();
            var apiaries = await _context.Apiaries
                .Include(h => h.Hives).ToListAsync();

            homeOverviewViewModel.ApiaryData = new List<ApiaryViewModel>();
            foreach (var apiary in apiaries)
            {
                var lastApiaryMeasurement = await _context.ApiaryMeasurement
                    .Where(m => m.ApiaryNumber == apiary.ApiaryNumber)
                    .OrderByDescending(m => m.MeasurementDate)
                    .FirstOrDefaultAsync();

                var activeApiaryMeasurement = lastApiaryMeasurement.MeasurementDate <= DateTime.UtcNow.AddMinutes(-20);

                var apiaryViewModel = new ApiaryViewModel()
                {
                    ApiaryId = apiary.ApiaryNumber,
                    Name = apiary.Name,
                    LastApiaryMeasurement = lastApiaryMeasurement,
                    Active = activeApiaryMeasurement
                };

                foreach (var hive in apiary.Hives)
                {
                    var hiveViewModel = new HiveViewModel()
                    {
                        HiveId = hive.HiveNumber,
                        HiveName = hive.Name
                    };

                    var hiveLastMeasurement = await _context.HiveMeasurement
                        .Where(m => m.HiveNumber == hive.HiveNumber)
                        .OrderByDescending(m => m.MeasurementDate)
                        .FirstOrDefaultAsync();

                    var activeHiveMeasurement = hiveLastMeasurement.MeasurementDate <= DateTime.UtcNow.AddMinutes(-20);
                    hiveViewModel.Active = activeHiveMeasurement;

                    hiveViewModel.LastHiveMeasurement = hiveLastMeasurement;
                    apiaryViewModel.Hives.Add(hiveViewModel);
                }
                homeOverviewViewModel.ApiaryData.Add(apiaryViewModel);
            }

            // Načtení počtů Apiaries a Hives
            var statisticsModel = new StatisticsModel
            {
                NumberOfApiaries = await _context.Apiaries.CountAsync(),
                NumberOfHives = await _context.Hives.CountAsync()
            };


            // Převod do ViewModelu
            var apiaryViewModels = apiaries.Select(a => new ApiaryViewModel
            {
                ApiaryId = a.ApiaryNumber,
                Name = a.Name,
                LastApiaryMeasurement = a.Measurements.OrderByDescending(m => m.MeasurementDate).FirstOrDefault(),
                Hives = a.Hives.Select(h => new HiveViewModel
                {
                    HiveId = h.HiveNumber,
                    HiveName = h.Name,
                    LastHiveMeasurement = h.Measurements.OrderByDescending(h => h.MeasurementDate).FirstOrDefault()
                }).ToList()
            }).ToList();

            homeOverviewViewModel.Statistics = statisticsModel;
            homeOverviewViewModel.ApiaryData = apiaryViewModels;

            stopwatch.Stop();
            ViewBag.LoadTimeMilliseconds = stopwatch.ElapsedMilliseconds;

            return View(homeOverviewViewModel);
        }
    }
}
