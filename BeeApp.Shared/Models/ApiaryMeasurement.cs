using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class ApiaryMeasurement
    {
        [Key]
        public int Id { get; set; }

        public DateTime MeasurementDate { get; set; }

        [ForeignKey("Apiary")]
        public int ApiaryId { get; set; }
        public Apiary Apiary { get; set; }

        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public int LightIntensity { get; set; }
    }
}
