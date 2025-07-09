using BeeApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeeApp.Web.Controllers
{
    public class SeederController : Controller
    {
        private readonly DataSeederService _seeder;

        public SeederController(DataSeederService seeder)
        {
            _seeder = seeder;
        }

        [HttpGet]
        public async Task<IActionResult> Run()
        {
            await _seeder.SeedFakeDataAsync();
            return Content("✅ Fake data seeded.");
        }
    }
}
