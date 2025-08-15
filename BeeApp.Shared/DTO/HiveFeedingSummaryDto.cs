using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class HiveFeedingSummaryDto
    {
        public int HiveId { get; set; }
        public string HiveName { get; set; } = "";
        public bool Started { get; set; }                 // true, pokud existuje aspoň 1 FeedingEvent

        public decimal? TotalSyrupLiters { get; set; }     // suma jen Syrup+Liter
        public decimal? TotalPattyGrams { get; set; }      // suma jen Patty+Gram
        // z plánu (může být null)
        public decimal? TargetSyrupLiters { get; set; }
        public decimal? TargetPattyGrams { get; set; }
        // dopočtené procenta vůči cíli
        public decimal? SyrupProgressPct { get; set; }
        public decimal? PattyProgressPct { get; set; }

        public DateTime? LastFedAt { get; set; }          // pro “poslední krmení”
        public int EventsCount { get; set; }
    }
}
