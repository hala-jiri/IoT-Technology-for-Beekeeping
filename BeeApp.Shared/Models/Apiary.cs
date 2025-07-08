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
    }
}
