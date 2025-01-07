using System.ComponentModel.DataAnnotations;

namespace ApiaryDataCollector.Models
{
    public class ApiaryMeasurement
    {
        [Key]
        public int Id { get; set; } // Primární klíč
        public int ApiaryNumber { get; set; } // Vazba na Apiary
        public Apiary? Apiary { get; set; } // Navigační vlastnost

        public DateTime MeasurementDate { get; set; } // Datum a čas měření

        public int Humidity { get; set; } // Vlhkost prostředí
        public double Temperature { get; set; } // Teplota prostředí
        public int LightIntensity { get; set; } // Světelná intenzita
    }
}
