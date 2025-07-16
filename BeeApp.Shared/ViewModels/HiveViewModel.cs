using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class HiveViewModel
    {
        public int HiveId { get; set; }
        public string Name { get; set; }
        public int ApiaryId { get; set; }
        public string ApiaryName { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public DateTime? LastMeasurement { get; set; }
        public double? LastWeight { get; set; }
        public double? LastTemperature { get; set; }
    }
}
