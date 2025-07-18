using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class HiveMiniChartViewModel
    {
        public int HiveId { get; set; }
        public string HiveName { get; set; }
        public string ApiaryName { get; set; }

        public string CurrentRange { get; set; } = "24h";
        public bool CurrentSmoothing { get; set; } = true;

        public List<HiveMiniPoint> DataPoints { get; set; } = new();
    }

    public class HiveMiniPoint
    {
        public DateTime Timestamp { get; set; }
        public double? Weight { get; set; }
        public double? Temperature { get; set; }
    }
}
