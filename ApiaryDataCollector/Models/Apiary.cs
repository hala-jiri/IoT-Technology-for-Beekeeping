using ApiaryDataCollector.Models;
using System.ComponentModel.DataAnnotations;

public class Apiary
{
    [Key]
    public int ApiaryNumber { get; set; } // Unikátní ID apiáře
    public string? Name { get; set; } // Název apiáře

    // Historická data měření
    public List<ApiaryMeasurement> Measurements { get; set; } = new List<ApiaryMeasurement>();

    public List<Hive> Hives { get; set; } = new List<Hive>();
}