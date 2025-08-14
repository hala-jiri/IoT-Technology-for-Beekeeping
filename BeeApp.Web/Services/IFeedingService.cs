using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;

namespace BeeApp.Web.Services
{
    public interface IFeedingService
    {
        Task<int> CreateAsync(FeedingEventCreateDto dto);
        Task UpdateAsync(int id, FeedingEventCreateDto dto);
        Task DeleteAsync(int id);

        Task<List<FeedingEvent>> GetEventsAsync(int hiveId, int seasonYear);
        Task<HiveFeedingSummaryDto> GetHiveSummaryAsync(int hiveId, int seasonYear);
        Task<List<HiveFeedingSummaryDto>> GetDashboardAsync(int? apiaryId, int seasonYear, DateTime? from = null, DateTime? to = null);

        Task<FeedingPlan?> GetPlanAsync(int hiveId, int seasonYear);
        Task UpsertPlanAsync(FeedingPlanUpsertDto dto);

        Task UpsertPlansBulkAsync(int seasonYear, IEnumerable<FeedingPlanBulkUpsertDto.Item> items);

    }
}
