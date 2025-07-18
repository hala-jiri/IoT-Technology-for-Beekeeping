﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.Models
{
    public class HiveMeasurement
    {
        [Key]
        public int Id { get; set; }

        public DateTime MeasurementDate { get; set; }

        [ForeignKey("Hive")]
        public int HiveId { get; set; }
        public Hive Hive { get; set; }

        public double Weight { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
    }
}
