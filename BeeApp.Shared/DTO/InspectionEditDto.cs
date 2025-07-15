using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class InspectionEditDto
    {
        public int InspectionReportId { get; set; }

        public int HiveId { get; set; }

        public DateTime InspectionDate { get; set; }

        public bool? QueenSeen { get; set; }
        public bool? BroodPresent { get; set; }
        public bool? EggsPresent { get; set; }
        public bool? PollenPresent { get; set; }
        public bool? HoneyPresent { get; set; }

        public string? Notes { get; set; }
    }
}
