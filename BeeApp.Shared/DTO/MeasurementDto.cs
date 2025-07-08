using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class MeasurementDto
    {
        public int ApiaryId { get; set; }
        public DateTime? MeasurementDate { get; set; }

        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public int LightIntensity { get; set; }

        public List<HiveMeasurementDto> Hives { get; set; }
    }
}
