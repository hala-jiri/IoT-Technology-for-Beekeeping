using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class InspectionReport
    {
        [Key]
        public int ReportId { get; set; }

        public DateTime InspectionDate { get; set; }

        [ForeignKey("Hive")]
        public int HiveId { get; set; }
        public Hive Hive { get; set; }

        public bool? QueenPresent { get; set; }
        public bool? SignsOfDisease { get; set; }
        public bool? AdequateFood { get; set; }
        public bool? HiveClean { get; set; }
        public bool? BroodPatternGood { get; set; }

        public string? Notes { get; set; }
    }
}
