namespace ApiaryDataWeb.Models
{
    public class HomeOverviewViewModel
    {
        public List<ApiaryViewModel> ApiaryData { get; set; } = new List<ApiaryViewModel>();
        public StatisticsModel Statistics { get; set; } = new StatisticsModel();
    }
}
