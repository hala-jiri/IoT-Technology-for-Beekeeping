using System.ComponentModel.DataAnnotations;

namespace ApiaryDataCollector.Models
{
    public class HiveMeasurement
    {
        [Key]
        public int Id { get; set; } // Primární klíč
        public int HiveNumber { get; set; } // Vazba na Hive
        public Hive? Hive { get; set; } // Navigační vlastnost

        public DateTime MeasurementDate { get; set; } // Datum a čas měření

        public double Weight { get; set; } // Hmotnost
        public double Temperature { get; set; } // Teplota
        public int Humidity { get; set; } // Vlhkost
    }
}
