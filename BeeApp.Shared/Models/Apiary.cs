using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class Apiary
    {
        [Key]
        public int ApiaryId { get; set; }

        public string? Name { get; set; }

        public List<Hive> Hives { get; set; } = new();
        public List<ApiaryMeasurement> Measurements { get; set; } = new();

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? ImageFileName { get; set; }  // Only name will be save

    }
}
