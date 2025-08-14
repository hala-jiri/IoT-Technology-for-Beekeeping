using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Shared.ViewModels
{
    public class HomeDashboardViewModel
    {
        public DashboardStats Stats { get; set; } = new DashboardStats();
        public List<DashboardAlert> Alerts { get; set; } = new List<DashboardAlert>();
        public List<RecentEventItem> RecentEvents { get; set; } = new List<RecentEventItem>();
        public List<QuickActionItem> QuickActions { get; set; } = new List<QuickActionItem>();

        public bool DataAvailable { get; set; } = true;      // když spadne DB, nastavíme na false
        public string? SystemMessage { get; set; }            // text „Data nejsou k dispozici…“
    }

    public class DashboardStats
    {
        public int ApiaryCount { get; set; }
        public int HiveCount { get; set; }
        public int ActiveHiveCount { get; set; }
        public int InspectionsLast30d { get; set; }
        public int FeedingsThisMonth { get; set; }
        public double? AvgWeightChange7dKg { get; set; }     // může být null
        public int AlertCount { get; set; }
    }

    //support classes for home dashboard
    public enum AlertType { OverdueInspection, WeightDrop, LowStores, Other }

    public class DashboardAlert
    {
        public AlertType Type { get; set; }
        public string Message { get; set; } = "";
        public int? HiveId { get; set; }
        //public string HiveName { get; set; }
        public string? ActionUrl { get; set; }
        public string Severity { get; set; } = "info";       // "info" | "warning" | "danger"
    }

    public enum EventKind { Inspection, Feeding, Milestone }

    public class RecentEventItem
    {
        public EventKind Kind { get; set; }
        public DateTime OccurredAt { get; set; }
        public int HiveId { get; set; }
        public string? Notes { get; set; }
        public string? DetailUrl { get; set; }
    }

    public enum QuickActionKind
    {
        AddInspection, AddFeeding, AddMilestone,
        TodayChecklist, WeightDropList, ScheduleInspectionsBulk, FeedingCalculator
    }

    public class QuickActionItem
    {
        public QuickActionKind Kind { get; set; }
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "⚙";
        public string ActionUrl { get; set; } = "#";
        public bool Highlight { get; set; }
    }
}
