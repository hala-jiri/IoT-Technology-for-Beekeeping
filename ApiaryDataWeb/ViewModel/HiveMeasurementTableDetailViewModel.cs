using ApiaryDataCollector.Models;
using ApiaryDataWeb.Models;

namespace ApiaryDataWeb.ViewModel
{
    public class HiveMeasurementTableDetailViewModel
    {
        public GlobalMinMaxHiveMesurements GlobalMinMaxHiveMesurements { get; set; }
        public List<HiveMeasurement> HiveMeasurementsList { get; set; }

        public int TotalRecords { get; set; } // Celkový počet záznamů
        public int CurrentPage { get; set; } // Aktuální stránka
        public int RowsPerPage { get; set; } // Počet řádků na stránku
    }
}
