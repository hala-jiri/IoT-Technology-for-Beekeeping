using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class HiveNote
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Hive")]
        public int HiveId { get; set; }
        public Hive Hive { get; set; }

        [Required]
        public string NoteText { get; set; }
    }
}
