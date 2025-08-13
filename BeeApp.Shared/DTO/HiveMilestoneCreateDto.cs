using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class HiveMilestoneCreateDto
    {
        public int HiveId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public MilestoneType Type { get; set; } = MilestoneType.Other;
    }
}
