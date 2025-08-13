using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class HiveMilestoneUpdateDto : HiveMilestoneCreateDto
    {
        public int HiveMilestoneId { get; set; }
    }
}
