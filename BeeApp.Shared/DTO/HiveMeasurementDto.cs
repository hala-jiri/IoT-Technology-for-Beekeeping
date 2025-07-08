using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class HiveMeasurementDto
    {
        public int HiveId { get; set; }
        public double? Weight { get; set; }
        public double? Temperature { get; set; }
        public int? Humidity { get; set; }
    }
}
