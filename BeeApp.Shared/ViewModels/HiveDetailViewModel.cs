using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class HiveDetailViewModel
    {
        public int HiveId { get; set; }
        public string HiveName { get; set; }

        public string ApiaryName { get; set; }
        public string ApiaryLocation { get; set; } // For now text - later map

        public DateTime? LastHiveMeasurementDate { get; set; }
        public double? LastWeight { get; set; }
        public double? LastHiveTemp { get; set; }

        public DateTime? LastApiaryMeasurementDate { get; set; }
        public double? LastApiaryTemp { get; set; }
        public double? LastApiaryPressure { get; set; }
        public double? LastApiaryLight { get; set; }
        
        public InspectionReport? LastInspection { get; set; }


        public List<HiveMeasurement> MeasurementsForChart { get; set; } = new();    // TODO: make changes that it load just last measurements
        public List<HiveMeasurementPoint> ChartData { get; set; } = new();

        public string CurrentRange { get; set; } = "14d";
        public int CurrentAggregation { get; set; }
        public bool CurrentSmoothing { get; set; } = true;
    }

    public class HiveMeasurementPoint
    {
        public DateTime Date { get; set; }
        public double? Weight { get; set; }
        public double? Temperature { get; set; }

        public double? ApiaryTemperature { get; set; }
    }
}
