using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class ApiaryViewModel
    {
        public int ApiaryId { get; set; }
        public string Name { get; set; }
        public int HiveCount { get; set; }
        public DateTime? LastMeasurement { get; set; }
    }
}
