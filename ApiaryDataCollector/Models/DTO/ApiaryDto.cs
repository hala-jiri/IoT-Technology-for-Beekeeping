namespace ApiaryDataCollector.Models.DTO
{
    public class ApiaryDto
    {
        public int ApiaryNumber { get; set; }
        public DateTime ReportDate { get; set; }
        public int Humidity { get; set; }
        public double Temperature { get; set; }
        public int LightIntensity { get; set; }
        public List<HiveDto>? Hives { get; set; }
    }
}
