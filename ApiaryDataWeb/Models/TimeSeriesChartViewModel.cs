namespace ApiaryDataWeb.Models
{
    public class TimeSeriesData
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
    }

    public class TimeSeriesChartViewModel
    {
        public List<List<TimeSeriesData>> DataSets { get; set; }
    }
}
