using Microsoft.EntityFrameworkCore;
using BeeApp.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BeeApp.Shared.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Apiary> Apiaries { get; set; }
        public DbSet<ApiaryMeasurement> ApiaryMeasurements { get; set; }
        public DbSet<Hive> Hives { get; set; }
        public DbSet<HiveMeasurement> HiveMeasurements { get; set; }
        public DbSet<HiveNote> HiveNotes { get; set; }
        public DbSet<InspectionReport> InspectionReports { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<WarehouseItemUsage> WarehouseItemUsages { get; set; }
    }
}
