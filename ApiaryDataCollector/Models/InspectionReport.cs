using System.ComponentModel.DataAnnotations;

namespace ApiaryDataCollector.Models
{
    public class InspectionReport
    {
        [Key]
        public int ReportId { get; set; }
        public DateTime InspectionDate { get; set; } // Datum inspekce

        public bool? QueenPresent { get; set; }
        public bool? SignsOfDisease { get; set; }
        public bool? AdequateFood { get; set; }
        public bool? HiveClean { get; set; }
        public bool? BroodPatternGood { get; set; }
    }
}
