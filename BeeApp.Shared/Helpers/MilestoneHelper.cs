using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Helpers
{
    public static class MilestoneHelper
    {
        public static string GetIcon(MilestoneType type) =>
            type switch
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
