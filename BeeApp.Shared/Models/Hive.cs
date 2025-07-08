using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class Hive
    {
        [Key]
        public int HiveId { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey("Apiary")]
        public int ApiaryId { get; set; }
        public Apiary Apiary { get; set; }

        public List<HiveMeasurement> Measurements { get; set; } = new();
        public List<InspectionReport> InspectionReports { get; set; } = new();
        public List<HiveNote> Notes { get; set; } = new();
    }
}
