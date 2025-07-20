using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class Backup
    {
        public int BackupId { get; set; }
        public DateTime Created { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string FileName { get; set; } = "";
        public long FileSize { get; set; }

        public DateTime? DataFrom { get; set; }
        public DateTime? DataTo { get; set; }
    }
}
