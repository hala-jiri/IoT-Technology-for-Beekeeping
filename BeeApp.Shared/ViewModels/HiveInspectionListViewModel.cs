using BeeApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class HiveInspectionListViewModel
    {
        public int HiveId { get; set; }
        public string HiveName { get; set; }
        public string ApiaryName { get; set; }

        public List<InspectionReport> Inspections { get; set; } = new();
    }
}
