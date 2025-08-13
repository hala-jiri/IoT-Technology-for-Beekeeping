using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class FeedingPlanUpsertDto
    {
        public int HiveId { get; set; }
        public int SeasonYear { get; set; }
        public decimal? TargetSyrupLiters { get; set; } // např. 15
        public decimal? TargetPattyGrams { get; set; } // např. 2000
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
