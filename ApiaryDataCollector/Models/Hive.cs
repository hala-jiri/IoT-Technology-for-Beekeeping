using ApiaryDataCollector.Models;
using System.ComponentModel.DataAnnotations;

public class Hive
{
    [Key]
    public int HiveNumber { get; set; } // Unikátní ID pro úl
    public string? Name { get; set; } // Název nebo popis úlu

    // Historická data měření
    public List<HiveMeasurement> Measurements { get; set; } = new List<HiveMeasurement>();

    // Inspekční zprávy
    public List<InspectionReport> InspectionReports { get; set; } = new List<InspectionReport>();
}