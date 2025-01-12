using ApiaryDataWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiaryDataWeb.Controllers
{
    public class ChartController : Controller
    {
        //https://www.chartjs.org/docs/latest/axes/cartesian/time.html
        /*
         *
         data: [{
                    x: "2016-12-25",
                    y: 3
                  }, {
                    x: "2016-12-28",
                    y: 10
                  }, {
                    x: "2016-12-29",
                    y: 5
                  }, {
                    x: "2016-12-30",
                    y: 2
                  }, {
                    x: "2017-01-03",
                    y: 20
                  }, {
                    x: "2017-01-05",
                    y: 30
                  }, {
                    x: "2017-01-08",
                    y: 45
            }], 
         */
        public IActionResult TimeSeriesChart()
        {
            ViewBag.ChartData = JsonConvert.SerializeObject(new
            {
                datasets = new[]
                {
                    new
                    {
                        label = "Teplota",
                        borderColor = "rgba(255, 99, 132, 1)", // Červená barva
                        data = new[]
                        {
                            new { koko = "2025-01-01T10:00:00Z", value = 10 }, // koko is the time
                            new { koko = "2025-01-01T11:00:00Z", value = 8 },
                            new { koko = "2025-01-01T12:00:00Z", value = 20 },
                            new { koko = "2025-01-01T13:00:00Z", value = 12 },
                            new { koko = "2025-01-01T14:00:00Z", value = 7 }
                        }
                    },
                    new
                    {
                        label = "Vlhkost",
                        borderColor = "rgba(54, 162, 235, 1)", // Modrá barva
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
            });

            // Pass raw JSON to the view
            ViewBag.TimeUnit = "hour";
            ViewBag.Minimum = 10-5;
            ViewBag.Maximum = 40+5;
            return View();
        }
    }
}
