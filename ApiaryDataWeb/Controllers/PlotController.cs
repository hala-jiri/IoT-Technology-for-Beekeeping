using ApiaryDataWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiaryDataWeb.Controllers
{
    public class ChartController : Controller
    {
        public IActionResult TimeSeriesChart()
        {
            var dataSet1 = new List<TimeSeriesData>
        {
            new TimeSeriesData { Time = new DateTime(2025, 1, 10, 12, 15, 0), Value = 5 },
            new TimeSeriesData { Time = new DateTime(2025, 1, 10, 12, 30, 0), Value = 12 },
            new TimeSeriesData { Time = new DateTime(2025, 1, 10, 12, 45, 0), Value = 13 }
        };

            var dataSet2 = new List<TimeSeriesData>
        {
            new TimeSeriesData { Time = new DateTime(2025, 1, 10, 12, 14, 0), Value = 21 },
            new TimeSeriesData { Time = new DateTime(2025, 1, 10, 12, 29, 0), Value = 23 },
            new TimeSeriesData { Time = new DateTime(2025, 1, 10, 12, 44, 0), Value = 18 }
        };

            var viewModel = new TimeSeriesChartViewModel
            {
                DataSets = new List<List<TimeSeriesData>> { dataSet1, dataSet2 }
            };

            return View(viewModel);
        }
    }
}
