using ApiaryDataWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiaryDataWeb.Controllers
{
    public class ChartController : Controller
    {
        public IActionResult TimeSeriesChart()
        {
            // Raw JSON data
            var rawJson = @"{
                ""dataSets"": [
                    [
                        { ""time"": ""2025-01-10T12:15:00"", ""value"": 40 },
                        { ""time"": ""2025-01-10T12:38:00"", ""value"": 38 },
                        { ""time"": ""2025-01-10T13:53:00"", ""value"": 13 }
                    ],
                    [
                        { ""time"": ""2025-01-10T12:14:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T12:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T12:44:00"", ""value"": 25 },
                        { ""time"": ""2025-01-10T12:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T13:14:00"", ""value"": 25 },
                        { ""time"": ""2025-01-10T13:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T13:44:00"", ""value"": 21 }
                    ]
                ]
            }";

            // Pass raw JSON to the view
            ViewBag.ChartData = rawJson;
            return View();
        }
    }
}
