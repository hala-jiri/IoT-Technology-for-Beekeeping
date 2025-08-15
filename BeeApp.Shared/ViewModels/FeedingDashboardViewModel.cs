using BeeApp.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class FeedingDashboardViewModel
    {
        public int SeasonYear { get; set; }
        public int? ApiaryId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public List<HiveFeedingSummaryDto> Hives { get; set; } = new();

        public decimal TotalSyrupLiters => Hives.Sum(h => h.TotalSyrupLiters) ?? 0m;
        public decimal TotalPattyGrams => Hives.Sum(h => h.TotalPattyGrams) ?? 0m;
    }
}
