using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class WarehouseItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int QuantityInStock { get; set; }

        public List<WarehouseItemUsage> Usages { get; set; } = new();
    }
}
