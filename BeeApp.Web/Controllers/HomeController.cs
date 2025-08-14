using BeeApp.Shared.ViewModels;
using BeeApp.Web.Models;
using BeeApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BeeApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeDashboardService _dashboard;

        public HomeController(IHomeDashboardService dashboard)
        {
            _dashboard = dashboard;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            HomeDashboardViewModel vm = await _dashboard.GetAsync();

            // Pokud jsou data nedostupná, zobrazíme hlášku na stránce (necháme i ve view)
            if (!vm.DataAvailable && !string.IsNullOrWhiteSpace(vm.SystemMessage))
            {
                TempData["DashboardWarning"] = vm.SystemMessage;
            }

            return View(vm);
        }

        // Volitelně: Privacy (ať ti funguje link v layoutu)
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
