using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiaryDataWeb.Controllers
{
    public class SeedDatabaseController : Controller
    {
        private readonly AppDbContext _context;

        public SeedDatabaseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            DateTime startOfProcess = DateTime.Now;
            DatabaseSeeder kuku = new DatabaseSeeder(_context);
            kuku.SeedDatabase(4,8,500,500,168);

            DateTime endOfProcess = DateTime.Now;

            TimeSpan timeOfSeeding = endOfProcess - startOfProcess;
            double seconds = Math.Round(timeOfSeeding.TotalSeconds, 2);
            ViewData["TimeOfSeeding"] = seconds.ToString();
            ViewData["CountOfApiaries"] = await _context.Apiaries.CountAsync();
            ViewData["CountOfHives"] = await _context.Hives.CountAsync();
            ViewData["CountOfApiaryMeasurements"] = await _context.ApiaryMeasurement.CountAsync();
            ViewData["CountOfHiveMeasurements"] = await _context.HiveMeasurement.CountAsync();
            return View();
        }


    }
}
