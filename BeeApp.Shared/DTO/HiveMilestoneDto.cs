using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class HiveMilestoneDto
    {
        public int HiveMilestoneId { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; } = string.Empty;
        public MilestoneType Type { get; set; }

        public string Icon =>
            Type switch
            {
                MilestoneType.Feeding => "🍽️",
                MilestoneType.Harvesting => "🍯",
                MilestoneType.Inspection => "🔍",
                MilestoneType.QueenAdded => "👑",
                MilestoneType.Treatment => "💊",
                _ => "📍"
            };
    }
}
