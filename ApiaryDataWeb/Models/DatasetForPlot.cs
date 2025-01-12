namespace ApiaryDataWeb.Models
{
    public class DatasetForPlot
    {
        public string? label { get; set; }
        public Data[]? data { get; set; }
        public string? borderColor { get; set; }
        public bool hidden { get; set; }
    }

    public class Data
    {
        public string? koko { get; set; }
        public float value { get; set; }
    }
}
