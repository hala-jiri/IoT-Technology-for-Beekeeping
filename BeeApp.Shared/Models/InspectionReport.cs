﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class InspectionReport
    {
        [Key]
        public int InspectionReportId { get; set; }

        public int HiveId { get; set; }
        public Hive Hive { get; set; }

        public DateTime InspectionDate { get; set; } = DateTime.Now;

        // data part
        public bool? QueenSeen { get; set; }
        public bool? BroodPresent { get; set; }
        public bool? EggsPresent { get; set; }
        public bool? PollenPresent { get; set; }
        public bool? HoneyPresent { get; set; }

        public string Notes { get; set; }
    }
}
