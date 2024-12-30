using ApiaryDataCollector.Models;
using Microsoft.EntityFrameworkCore;
using System;

public class AppDbContext : DbContext
{
    public DbSet<Apiary> Apiaries { get; set; }
    public DbSet<Hive> Hives { get; set; }
    public DbSet<ApiaryMeasurement> ApiaryMeasurement { get; set; }
    public DbSet<HiveMeasurement> HiveMeasurement { get; set; }
    public DbSet<InspectionReport> InspectionReport { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apiary>().HasKey(a => a.ApiaryNumber);
        modelBuilder.Entity<Hive>().HasKey(h => h.HiveNumber);
        modelBuilder.Entity<InspectionReport>().HasKey(i => i.ReportId);
        modelBuilder.Entity<HiveMeasurement>().HasKey(h => h.Id);
        modelBuilder.Entity<ApiaryMeasurement>().HasKey(a => a.Id);

        // Vazby mezi Apiary a ApiaryMeasurement
        modelBuilder.Entity<ApiaryMeasurement>()
            .HasOne(am => am.Apiary)
            .WithMany(a => a.Measurements)
            .HasForeignKey(am => am.ApiaryNumber);


        // Vazby mezi Hive a HiveMeasurement
        modelBuilder.Entity<HiveMeasurement>()
            .HasOne(hm => hm.Hive)
            .WithMany(h => h.Measurements)
            .HasForeignKey(hm => hm.HiveNumber);

        // Vazby mezi Hive a InspectionReport
        modelBuilder.Entity<InspectionReport>()
            .HasOne<Hive>() // InspectionReport není přímo vázán přes Hive ID, ale pokud je, přidejte navigační vlastnost.
            .WithMany(h => h.InspectionReports);
    }
}
