using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class HiveFeedingIndexViewModel
    {
        public int HiveId { get; set; }
        public string HiveName { get; set; } = "";
        public int SeasonYear { get; set; }

        public HiveFeedingSummaryDto Summary { get; set; } = default!;
        public List<FeedingEvent> Events { get; set; } = new();

        public FeedingEventCreateDto QuickAdd { get; set; } = new FeedingEventCreateDto();
    }
}
