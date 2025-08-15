using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.ViewModels;
using BeeApp.Web.Data;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeeApp.Web.Services
{
    public class HomeDashboardService : IHomeDashboardService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeDashboardService> _logger;
        private readonly IFeedingService _feedingService;

        public HomeDashboardService(AppDbContext context, ILogger<HomeDashboardService> logger, IFeedingService feedingService)
        {
            _context = context;
            _logger = logger;
            _feedingService = feedingService;
        }

        public async Task<HomeDashboardViewModel> GetAsync()
        {
            var vm = new HomeDashboardViewModel();

            try
            {
                // --- Statistiky ---
                vm.Stats.ApiaryCount = await _context.Apiaries.CountAsync();
                vm.Stats.HiveCount = await _context.Hives.CountAsync();
                vm.Stats.ActiveHiveCount = vm.Stats.HiveCount;  //await _context.Hives.CountAsync(h => h.IsActive); // doesnt have yet ActiveStatus

                var d30 = DateTime.Today.AddDays(-30);
                var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                vm.Stats.InspectionsLast30d = await _context.InspectionReports.CountAsync(i => i.InspectionDate >= d30);
                vm.Stats.FeedingsThisMonth = await _context.FeedingEvents.CountAsync(f => f.Date >= monthStart);


                // Stats about feeding
                decimal feedingRemainingL = 0m;
                decimal feedingRemainingKg = 0m;
                var hiveIds = await _context.Hives.Select(h => h.HiveId).ToListAsync();
                try
                {
                    int seasonYear = DateTime.Today.Year;
                    foreach (var id in hiveIds)
                    {
                        var s = await _feedingService.GetHiveSummaryAsync(id, seasonYear)
                                    ?? new HiveFeedingSummaryDto();

                        var remainLiters = (decimal)((s.TargetSyrupLiters ?? 0m) - (s.TotalSyrupLiters ?? 0m));
                        var remainPattyKg = ((decimal)((s.TargetPattyGrams ?? 0m) - (s.TotalPattyGrams ?? 0m))) / 1000m;

                        if (remainLiters > 0) feedingRemainingL += remainLiters;
                        if (remainPattyKg > 0) feedingRemainingKg += remainPattyKg;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Feeding remain calculation failed – continuing with zeros.");
                    feedingRemainingL = 0m;
                    feedingRemainingKg = 0m;
                }

                // ulož do Stats
                vm.Stats.FeedingRemainingLiters = (double)Math.Round(feedingRemainingL, 1);
                vm.Stats.FeedingRemainingKg = (double)Math.Round(feedingRemainingKg, 1);

                // Volitelná průměrná změna váhy za 7 dní – jednoduchá verze (když nejsou data, zůstane null)
                var d7 = DateTime.Today.AddDays(-7);
                if (await _context.HiveMeasurements.AnyAsync())
                {
                    // průměr (poslední váha - váha před 7 dny) napříč úly
                    //var hiveIds = await _context.Hives.Select(h => h.HiveId).ToListAsync();
                    double total = 0; int cnt = 0;
                    foreach (var id in hiveIds)
                    {
                        var last = await _context.HiveMeasurements
                            .Where(m => m.HiveId == id)
                            .OrderByDescending(m => m.MeasurementDate)
                            .Select(m => (double?)m.Weight)
                            .FirstOrDefaultAsync();

                        var old = await _context.HiveMeasurements
                            .Where(m => m.HiveId == id && m.MeasurementDate <= d7)
                            .OrderByDescending(m => m.MeasurementDate)
                            .Select(m => (double?)m.Weight)
                            .FirstOrDefaultAsync();

                        if (last.HasValue && old.HasValue)
                        {
                            total += (last.Value - old.Value);
                            cnt++;
                        }
                    }
                    if (cnt > 0) vm.Stats.AvgWeightChange7dKg = total / cnt;
                }

                // --- Notifikace (jednoduchá pravidla s pevnými hodnotami) ---
                int overdueDays = 14;
                double weightDropKg = 2.0;
                double lowStoresKg = 6.0;

                var overdueDate = DateTime.Today.AddDays(-overdueDays);

                // 1) Bez inspekce déle než 14 dní
                var hivesWithLastInspection = await _context.Hives
                    .Select(h => new
                    {
                        h.HiveId,
                        LastInspection = _context.InspectionReports
                            .Where(i => i.HiveId == h.HiveId)
                            .OrderByDescending(i => i.InspectionDate)
                            .Select(i => (DateTime?)i.InspectionDate)
                            .FirstOrDefault()
                    }).ToListAsync();

                foreach (var h in hivesWithLastInspection)
                {
                    if (!h.LastInspection.HasValue || h.LastInspection.Value.Date < overdueDate)
                    {
                        vm.Alerts.Add(new DashboardAlert
                        {
                            Type = AlertType.OverdueInspection,
                            Severity = "warning",
                            HiveId = h.HiveId,
                            Message = $"Úl #{h.HiveId} nebyl kontrolován {overdueDays}+ dní.",
                            ActionUrl = $"/Hive/Detail/{h.HiveId}"
                        });
                    }
                }

                // 2) Pokles váhy ≥ 2 kg za 7 dní (pokud jsou měření)
                if (await _context.HiveMeasurements.AnyAsync())
                {
                    var ids = await _context.Hives.Select(h => h.HiveId).ToListAsync();
                    foreach (var id in ids)
                    {
                        var last = await _context.HiveMeasurements
                            .Where(m => m.HiveId == id)
                            .OrderByDescending(m => m.MeasurementDate)
                            .Select(m => (double?)m.Weight)
                            .FirstOrDefaultAsync();

                        var old = await _context.HiveMeasurements
                            .Where(m => m.HiveId == id && m.MeasurementDate <= d7)
                            .OrderByDescending(m => m.MeasurementDate)
                            .Select(m => (double?)m.Weight)
                            .FirstOrDefaultAsync();

                        if (last.HasValue && old.HasValue && (last.Value - old.Value) <= -weightDropKg)
                        {
                            vm.Alerts.Add(new DashboardAlert
                            {
                                Type = AlertType.WeightDrop,
                                Severity = "danger",
                                HiveId = id,
                                Message = $"Úl #{id} má pokles ≥ {weightDropKg} kg za 7 dní.",
                                ActionUrl = $"/Hive/Detail/{id}"
                            });
                        }
                    }
                }

                // 3) Nízké zásoby (poslední váha < 6 kg) – orientačně
                if (await _context.HiveMeasurements.AnyAsync())
                {
                    var ids = await _context.Hives.Select(h => h.HiveId).ToListAsync();
                    foreach (var id in ids)
                    {
                        var last = await _context.HiveMeasurements
                            .Where(m => m.HiveId == id)
                            .OrderByDescending(m => m.MeasurementDate)
                            .Select(m => (double?)m.Weight)
                            .FirstOrDefaultAsync();

                        if (last.HasValue && last.Value < lowStoresKg)
                        {
                            vm.Alerts.Add(new DashboardAlert
                            {
                                Type = AlertType.LowStores,
                                Severity = "warning",
                                HiveId = id,
                                Message = $"Úl #{id} má málo zásob (< {lowStoresKg} kg).",
                                ActionUrl = $"/Feeding/Create?hiveId={id}"
                            });
                        }
                    }
                }

                vm.Stats.AlertCount = vm.Alerts.Count;

                // --- Poslední události (poskládané dohromady a seřazené) ---
                var recentInspections = await _context.InspectionReports
                    .OrderByDescending(i => i.InspectionDate).Take(5)
                    .Select(i => new RecentEventItem
                    {
                        Kind = EventKind.Inspection,
                        OccurredAt = i.InspectionDate,
                        HiveId = i.HiveId,
                        DetailUrl = $"/Inspection/Detail/{i.HiveId}"
                    }).ToListAsync();

                var recentFeedings = await _context.FeedingEvents
                    .OrderByDescending(f => f.Date).Take(5)
                    .Select(f => new RecentEventItem
                    {
                        Kind = EventKind.Feeding,
                        OccurredAt = f.Date,
                        HiveId = f.HiveId,
                        Notes = $"{f.Quantity} l sirupu",
                        DetailUrl = $"/Feeding/Detail/{f.Id}"
                    }).ToListAsync();

                var recentMilestones = await _context.HiveMilestones
                    .OrderByDescending(m => m.Date).Take(5)
                    .Select(m => new RecentEventItem
                    {
                        Kind = EventKind.Milestone,
                        OccurredAt = m.Date,
                        HiveId = m.HiveId,
                        Notes = m.Type.ToString(),
                        DetailUrl = $"/HiveMilestone/Detail/{m.HiveId}"
                    }).ToListAsync();

                vm.RecentEvents = recentInspections
                    .Concat(recentFeedings)
                    .Concat(recentMilestones)
                    .OrderByDescending(e => e.OccurredAt)
                    .Take(10)
                    .ToList();

                // --- Rychlé akce (jednoduše) ---
                vm.QuickActions.Add(new QuickActionItem { Kind = QuickActionKind.AddInspection, Label = "Přidat inspekci", Icon = "📋", ActionUrl = "/Inspection/Create" });
                vm.QuickActions.Add(new QuickActionItem { Kind = QuickActionKind.AddFeeding, Label = "Přidat krmení", Icon = "🍯", ActionUrl = "/Feeding/Create" });
                vm.QuickActions.Add(new QuickActionItem { Kind = QuickActionKind.AddMilestone, Label = "Přidat milestone", Icon = "📌", ActionUrl = "/HiveMilestone/Create" });

                // zvýrazni akce podle alertů (velmi jednoduše)
                if (vm.Alerts.Any(a => a.Type == AlertType.WeightDrop))
                    vm.QuickActions.Insert(0, new QuickActionItem { Kind = QuickActionKind.WeightDropList, Label = "Úly s poklesem váhy", Icon = "📉", ActionUrl = "/Hive?filter=weightdrop", Highlight = true });

                if (vm.Alerts.Any(a => a.Type == AlertType.OverdueInspection))
                    vm.QuickActions.Insert(0, new QuickActionItem { Kind = QuickActionKind.ScheduleInspectionsBulk, Label = "Naplánovat inspekce", Icon = "🗓️", ActionUrl = "/Inspection/BulkPlan", Highlight = true });

                if (vm.Alerts.Any(a => a.Type == AlertType.LowStores))
                    vm.QuickActions.Insert(0, new QuickActionItem { Kind = QuickActionKind.FeedingCalculator, Label = "Kalkulačka krmení", Icon = "🧮", ActionUrl = "/Feeding/Calculator", Highlight = true });

                vm.DataAvailable = true;
            }
            catch (Exception ex)
            {
                // Když DB spadne, pořád vrátíme stránku s prázdnými bloky a varováním
                _logger.LogError(ex, "Dashboard load failed.");
                vm.DataAvailable = false;
                vm.SystemMessage = "Data nejsou momentálně k dispozici (databáze nedostupná).";
                vm.Stats = new DashboardStats();
                vm.Alerts = new List<DashboardAlert>();
                vm.RecentEvents = new List<RecentEventItem>();
                vm.QuickActions = new List<QuickActionItem>
                {
                new QuickActionItem { Kind = QuickActionKind.AddInspection, Label = "Přidat inspekci", Icon = "📋", ActionUrl = "/Inspection/Create" },
                new QuickActionItem { Kind = QuickActionKind.AddFeeding, Label = "Přidat krmení", Icon = "🍯", ActionUrl = "/Feeding/Create" },
                new QuickActionItem { Kind = QuickActionKind.AddMilestone, Label = "Přidat milestone", Icon = "📌", ActionUrl = "/HiveMilestone/Create" }
                };
            }

            return vm;
        }
    }
}
