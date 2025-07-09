using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.DTO
{
    public class HiveUpdateDto
    {
        public int HiveId { get; set; }

        [Required]
        public string Name { get; set; }

        public int ApiaryId { get; set; }
    }
}
