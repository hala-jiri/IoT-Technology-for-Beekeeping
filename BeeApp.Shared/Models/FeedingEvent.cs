using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public enum FeedingMedium { Syrup = 0, Patty = 1 } // Patty = těsto (fondant/pollen patty)
    public enum FeedingUnit { Liter = 0, Gram = 1 }

    public class FeedingEvent
    {
        public int Id { get; set; }
        public int HiveId { get; set; }
        public DateTime Date { get; set; }

        public FeedingMedium Medium { get; set; } // Syrup / Patty
        public decimal Quantity { get; set; }     // hodnota
        public FeedingUnit Unit { get; set; }     // L nebo g

        // Volitelné doplňky
        public string? SyrupRatio { get; set; }   // např. "3:2" (jen pro Syrup)
        public string? Additives { get; set; }    // např. "Invertofix"
        public string? Note { get; set; }

        // Napojení
        public int? InspectionId { get; set; }
        public Hive Hive { get; set; } = default!;
    }

    public class FeedingPlan // cíle na sezónu pro přehled/progress
    {
        public int Id { get; set; }
        public int HiveId { get; set; }
        public int SeasonYear { get; set; }

        // cíle – oddělené podle média/jednotek
        public decimal? TargetSyrupLiters { get; set; }   // např. 15 L
        public decimal? TargetPattyGrams { get; set; }    // např. 2000 g

        // volitelně období, ve kterém plán plníš (pro “on track” výpočet)
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
    }
}
