using ApiaryDataCollector.Models;

namespace ApiaryDataWeb.Models
{
    public class HiveViewModel
    {
        public int HiveId { get; set; }
        public string? HiveName { get; set; }
        public HiveMeasurement? LastHiveMeasurement { get; set; }
        public bool Active { get; set; } //just to see if the last measuremnt was too far ago.
        public InspectionReport? LastInspectionReport { get; set; }
    }
}
