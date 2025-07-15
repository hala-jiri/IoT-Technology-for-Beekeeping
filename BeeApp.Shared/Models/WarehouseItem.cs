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
        public int WarehouseItemId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;  // např. "Stropní krmítko"

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Available;

        public string? LocationNote { get; set; } // např. "Apiary 1", "sklad 2"

        public DateTime Created { get; set; } = DateTime.Now;
    }

    public enum ItemStatus
    {
        Available = 0,
        InUse = 1,
        Damaged = 2,
        Discarded = 3
    }
}
