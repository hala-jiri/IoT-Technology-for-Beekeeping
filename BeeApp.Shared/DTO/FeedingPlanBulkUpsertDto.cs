using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class FeedingPlanBulkUpsertDto
    {
        public int SeasonYear { get; set; }
        public int? ApiaryId { get; set; } // volitelný filtr z UI
        public List<Item> Items { get; set; } = new();

        public class Item
        {
            public bool Selected { get; set; }
            public int HiveId { get; set; }
            public string HiveName { get; set; } = ""; // jen pro zobrazení zpět při chybách
            public decimal? TargetSyrupLiters { get; set; }
            public decimal? TargetPattyGrams { get; set; }
            // volitelně:
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
        }
    }
}
