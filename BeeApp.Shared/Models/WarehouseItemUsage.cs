using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class WarehouseItemUsage
    {
        [Key]
        public int UsageId { get; set; }

        [ForeignKey("WarehouseItem")]
        public int ItemId { get; set; }
        public WarehouseItem WarehouseItem { get; set; }

        public int? HiveId { get; set; }
        public Hive? Hive { get; set; }

        public int QuantityUsed { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "used"; // "used", "missing", "in-stock"
    }
}
