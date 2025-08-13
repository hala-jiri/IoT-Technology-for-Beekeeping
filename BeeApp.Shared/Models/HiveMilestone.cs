using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class HiveMilestone
    {
        public int HiveMilestoneId { get; set; }
        public int HiveId { get; set; }
        public Hive Hive { get; set; } = null!;
        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; } = string.Empty;
        public MilestoneType Type { get; set; } = MilestoneType.Other;
    }


    public enum MilestoneType
    {
        Feeding,
        Harvesting,
        Inspection,
        QueenAdded,
        Treatment,
        Other
    }
}
