using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class ExportDataRequestViewModel
    {
        public int SelectedApiaryId { get; set; }
        public List<int> SelectedHiveIds { get; set; } = new();
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool IncludeMeasurements { get; set; } = true;
        public bool IncludeInspections { get; set; } = false;

        public List<Apiary> AvailableApiaries { get; set; } = new();
        public List<Hive> AvailableHives { get; set; } = new();
    }
}
