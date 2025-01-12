using ApiaryDataWeb.Models;
using Microsoft.AspNetCore.Mvc;

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
                        { ""time"": ""2025-01-10T13:44:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T13:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T13:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T14:14:00"", ""value"": 25 },
                        { ""time"": ""2025-01-10T14:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T14:44:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T14:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T15:14:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T15:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T15:44:00"", ""value"": 25 },
                        { ""time"": ""2025-01-10T15:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T16:14:00"", ""value"": 25 },
                        { ""time"": ""2025-01-10T16:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T16:44:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T16:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T17:14:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T17:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T17:44:00"", ""value"": 25 },
                        { ""time"": ""2025-01-10T17:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T18:14:00"", ""value"": 25 },
                        { ""time"": ""2025-01-10T18:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T18:44:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T18:59:00"", ""value"": 27 },
                        { ""time"": ""2025-01-10T19:14:00"", ""value"": 21 },
                        { ""time"": ""2025-01-10T19:29:00"", ""value"": 23 },
                        { ""time"": ""2025-01-10T19:44:00"", ""value"": 25 },
                       ]
                ]}";

            // Pass raw JSON to the view
            ViewBag.TimeUnit = "minute";
            ViewBag.Minimum = 10-5;
            ViewBag.Maximum = 40+5;
            ViewBag.ChartData = rawJson;
            return View();
        }
    }
}
