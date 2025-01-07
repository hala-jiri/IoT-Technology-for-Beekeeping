namespace ApiaryDataCollector.Models.DTO
{
    public class HiveSummaryDto
    {
        public int HiveNumber { get; set; }
        public string? Name { get; set; }
        public int ApiaryNumber { get; set; }
        public required string ApiaryName { get; set; }
        // Přidej další vlastnosti, pokud jsou potřeba
    }
}