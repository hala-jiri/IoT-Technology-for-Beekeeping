namespace ApiaryDataWeb.Models
{
    public class DatasetForPlot
    {
        public string? label { get; set; }
        public double[]? data { get; set; }
        public bool fill { get; set; }
        public string? borderColor { get; set; }
        public double tension { get; set; }
    }
}
