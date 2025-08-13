using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Services
{
    public class FeedingService : IFeedingService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FeedingService> _logger;

        public FeedingService(AppDbContext context, ILogger<FeedingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CreateAsync(FeedingEventCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(dto.Quantity), "Quantity must be > 0.");

            var unit = dto.Medium == FeedingMedium.Syrup ? FeedingUnit.Liter : FeedingUnit.Gram;

            var e = new FeedingEvent
            {
                HiveId = dto.HiveId,
                Date = dto.Date,
                Medium = dto.Medium,
                Quantity = dto.Quantity,
                Unit = unit,
                SyrupRatio = dto.SyrupRatio,
                Additives = dto.Additives,
                Note = dto.Note,
                InspectionId = dto.InspectionId
            };

            _context.FeedingEvents.Add(e);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created FeedingEvent {Id} for Hive {HiveId} on {Date}", e.Id, e.HiveId, e.Date);
            return e.Id;
        }

        public async Task UpdateAsync(int id, FeedingEventCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(dto.Quantity), "Quantity must be > 0.");

            var e = await _context.FeedingEvents.FindAsync(id);
            if (e == null)
            {
                _logger.LogWarning("Update failed: FeedingEvent {Id} not found", id);
                throw new KeyNotFoundException("Feeding event not found.");
            }

            e.Date = dto.Date;
            e.Medium = dto.Medium;
            e.Unit = dto.Medium == FeedingMedium.Syrup ? FeedingUnit.Liter : FeedingUnit.Gram;
            e.Quantity = dto.Quantity;
            e.SyrupRatio = dto.SyrupRatio;
            e.Additives = dto.Additives;
            e.Note = dto.Note;
            e.InspectionId = dto.InspectionId;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated FeedingEvent {Id}", id);
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _context.FeedingEvents.FindAsync(id);
            if (e == null)
            {
                _logger.LogWarning("Delete ignored: FeedingEvent {Id} not found", id);
                return;
            }

            _context.FeedingEvents.Remove(e);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted FeedingEvent {Id}", id);
        }

        public async Task<List<FeedingEvent>> GetEventsAsync(int hiveId, int seasonYear)
        {
            return await _context.FeedingEvents
               .AsNoTracking()
               .Where(x => x.HiveId == hiveId && x.Date.Year == seasonYear)
               .OrderByDescending(x => x.Date)
               .ToListAsync();
        }

        public async Task<HiveFeedingSummaryDto> GetHiveSummaryAsync(int hiveId, int seasonYear)
        {
            var agg = await _context.FeedingEvents
                .AsNoTracking() // faster reading, no tracking of changes, just read and thats all. Maybe should use for plot (chart)
                .Where(f => f.HiveId == hiveId && f.Date.Year == seasonYear)
                .GroupBy(f => f.HiveId)
                .Select(g => new
                {
                    TotalSyrupLiters = g.Where(x => x.Medium == FeedingMedium.Syrup && x.Unit == FeedingUnit.Liter).Sum(x => x.Quantity),
                    TotalPattyGrams = g.Where(x => x.Medium == FeedingMedium.Patty && x.Unit == FeedingUnit.Gram).Sum(x => x.Quantity),
                    LastFedAt = g.Max(x => (DateTime?)x.Date),
                    EventsCount = g.Count()
                })
                .FirstOrDefaultAsync();

            var hive = await _context.Hives
                .AsNoTracking()
                .Where(h => h.HiveId == hiveId)
                .Select(h => new { h.HiveId, h.Name })
                .FirstAsync();

            var plan = await _context.FeedingPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.HiveId == hiveId && p.SeasonYear == seasonYear);

            return new HiveFeedingSummaryDto
            {
                HiveId = hive.HiveId,
                HiveName = hive.Name,
                Started = agg != null && agg.EventsCount > 0,
                TotalSyrupLiters = agg?.TotalSyrupLiters ?? 0,
                TotalPattyGrams = agg?.TotalPattyGrams ?? 0,
                TargetSyrupLiters = plan?.TargetSyrupLiters,
                TargetPattyGrams = plan?.TargetPattyGrams,
                SyrupProgressPct = (plan?.TargetSyrupLiters > 0)
                    ? Math.Round(100m * (agg?.TotalSyrupLiters ?? 0) / plan!.TargetSyrupLiters!.Value, 1)
                    : null,
                PattyProgressPct = (plan?.TargetPattyGrams > 0)
                    ? Math.Round(100m * (agg?.TotalPattyGrams ?? 0) / plan!.TargetPattyGrams!.Value, 1)
                    : null,
                LastFedAt = agg?.LastFedAt,
                EventsCount = agg?.EventsCount ?? 0
            };
        }

        public async Task<List<HiveFeedingSummaryDto>> GetDashboardAsync(int? apiaryId, int seasonYear, DateTime? from = null, DateTime? to = null)
        {
            // 1) Hives (filtrováno dle včelnice), seřazené podle jména
            var hives = await _context.Hives
                .AsNoTracking()
                .Where(h => !apiaryId.HasValue || h.ApiaryId == apiaryId.Value)
                .Select(h => new { h.HiveId, h.Name })
                .OrderBy(h => h.Name)
                .ToListAsync();

            // 2) Agregace krmení pro daný rok (+ volitelné datum od/do)
            var feedQuery = _context.FeedingEvents.AsNoTracking()
                .Where(f => f.Date.Year == seasonYear);
            if (from.HasValue) feedQuery = feedQuery.Where(f => f.Date >= from.Value);
            if (to.HasValue) feedQuery = feedQuery.Where(f => f.Date <= to.Value);

            var feedAgg = await feedQuery
                .GroupBy(f => f.HiveId)
                .Select(g => new FeedAggRow
                {
                    HiveId = g.Key,
                    TotalSyrupLiters = g.Where(x => x.Medium == FeedingMedium.Syrup && x.Unit == FeedingUnit.Liter).Sum(x => x.Quantity),
                    TotalPattyGrams = g.Where(x => x.Medium == FeedingMedium.Patty && x.Unit == FeedingUnit.Gram).Sum(x => x.Quantity),
                    LastFedAt = g.Max(x => (DateTime?)x.Date),
                    EventsCount = g.Count()
                })
                .ToListAsync();

            var feedByHive = feedAgg.ToDictionary(x => x.HiveId);

            // 3) Plány na daný rok
            var plans = await _context.FeedingPlans
                .AsNoTracking()
                .Where(p => p.SeasonYear == seasonYear)
                .Select(p => new { p.HiveId, p.TargetSyrupLiters, p.TargetPattyGrams })
                .ToListAsync();

            var planByHive = plans.ToDictionary(x => x.HiveId);

            // 4) Složení výsledků bez joinů – jen Dictionary lookups
            var result = new List<HiveFeedingSummaryDto>(hives.Count);

            foreach (var h in hives)
            {
                feedByHive.TryGetValue(h.HiveId, out var f);
                planByHive.TryGetValue(h.HiveId, out var p);

                var totalSyrup = f?.TotalSyrupLiters ?? 0m;
                var totalPatty = f?.TotalPattyGrams ?? 0m;

                decimal? syrupPct = (p?.TargetSyrupLiters > 0)
                    ? Math.Round(100m * totalSyrup / p!.TargetSyrupLiters!.Value, 1)
                    : (decimal?)null;

                decimal? pattyPct = (p?.TargetPattyGrams > 0)
                    ? Math.Round(100m * totalPatty / p!.TargetPattyGrams!.Value, 1)
                    : (decimal?)null;

                result.Add(new HiveFeedingSummaryDto
                {
                    HiveId = h.HiveId,
                    HiveName = h.Name,
                    Started = (f?.EventsCount ?? 0) > 0,
                    TotalSyrupLiters = totalSyrup,
                    TotalPattyGrams = totalPatty,
                    TargetSyrupLiters = p?.TargetSyrupLiters,
                    TargetPattyGrams = p?.TargetPattyGrams,
                    SyrupProgressPct = syrupPct,
                    PattyProgressPct = pattyPct,
                    LastFedAt = f?.LastFedAt,
                    EventsCount = f?.EventsCount ?? 0
                });
            }

            return result;
        }

        public async Task<FeedingPlan?> GetPlanAsync(int hiveId, int seasonYear)
        {
            return await _context.FeedingPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.HiveId == hiveId && p.SeasonYear == seasonYear);
        }

        public async Task UpsertPlanAsync(FeedingPlanUpsertDto dto)
        {
            if (dto.HiveId <= 0) throw new ArgumentOutOfRangeException(nameof(dto.HiveId));
            if (dto.SeasonYear <= 0) throw new ArgumentOutOfRangeException(nameof(dto.SeasonYear));
            if (dto.TargetSyrupLiters < 0 || dto.TargetPattyGrams < 0)
                throw new ArgumentOutOfRangeException("Targets must be >= 0");

            var plan = await _context.FeedingPlans
                .FirstOrDefaultAsync(p => p.HiveId == dto.HiveId && p.SeasonYear == dto.SeasonYear);

            if (plan == null)
            {
                plan = new FeedingPlan
                {
                    HiveId = dto.HiveId,
                    SeasonYear = dto.SeasonYear,
                    TargetSyrupLiters = dto.TargetSyrupLiters,
                    TargetPattyGrams = dto.TargetPattyGrams,
                    From = dto.From != null ? DateOnly.FromDateTime(dto.From.Value) : null,
                    To = dto.To != null ? DateOnly.FromDateTime(dto.To.Value) : null
                };
                _context.FeedingPlans.Add(plan);
            }
            else
            {
                plan.TargetSyrupLiters = dto.TargetSyrupLiters;
                plan.TargetPattyGrams = dto.TargetPattyGrams;
                plan.From = dto.From != null ? DateOnly.FromDateTime(dto.From.Value) : null;
                plan.To = dto.To != null ? DateOnly.FromDateTime(dto.To.Value) : null;
            }

            await _context.SaveChangesAsync();
        }

        private sealed class FeedAggRow
        {
            public int HiveId { get; set; }
            public decimal TotalSyrupLiters { get; set; }
            public decimal TotalPattyGrams { get; set; }
            public DateTime? LastFedAt { get; set; }
            public int EventsCount { get; set; }
        }
    }
}
