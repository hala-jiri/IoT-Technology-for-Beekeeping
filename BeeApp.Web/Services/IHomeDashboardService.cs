using BeeApp.Shared.ViewModels;

namespace BeeApp.Web.Services
{
    public interface IHomeDashboardService
    {
        Task<HomeDashboardViewModel> GetAsync();
    }
}
