using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class FeedingEventCreateDto
    {
        public int HiveId { get; set; }
        public DateTime Date { get; set; }
        public FeedingMedium Medium { get; set; } // Syrup/Patty
        public decimal Quantity { get; set; }     // L nebo g
        public FeedingUnit Unit { get; set; }     // Liter/Gram
        public string? SyrupRatio { get; set; }
        public string? Additives { get; set; }
        public string? Note { get; set; }
        public int? InspectionId { get; set; }
    }
}
