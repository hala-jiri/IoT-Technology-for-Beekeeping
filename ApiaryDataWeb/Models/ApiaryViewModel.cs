using ApiaryDataCollector.Models;

namespace ApiaryDataWeb.Models
{
    public class ApiaryViewModel
    {
        public int ApiaryId { get; set; }
        public string? Name { get; set; }
        public ApiaryMeasurement? LastApiaryMeasurement { get; set; }
        public bool Active { get; set; } //just to see if the last measuremnt was too far ago.
        public List<HiveViewModel> Hives { get; set; } = new List<HiveViewModel>();
    }
}
