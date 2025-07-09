using BeeApp.Shared.Data;
using BeeApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeApp.Web.Services
{
    public class DataSeederService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DataSeederService> _logger;

        public DataSeederService(AppDbContext context, ILogger<DataSeederService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedFakeDataAsync()
        {
            if (await _context.Apiaries.AnyAsync(a => a.Name.StartsWith("Fake Apiary")))
            {
                _logger.LogInformation("Fake data already exists. Skipping seeding.");
                return;
            }

            var apiaries = new List<Apiary>();
            for (int i = 1; i <= 2; i++)
            {
                apiaries.Add(new Apiary
                {
                    Name = $"Fake Apiary {i}",
                    //TODO: Location = $"Test Location {i}",
                    Hives = new List<Hive>()
                });
            }

            _context.Apiaries.AddRange(apiaries);
            await _context.SaveChangesAsync();

            int hiveCounter = 1;
            foreach (var apiary in apiaries)
            {
                for (int j = 1; j <= 3; j++)
                {
                    var hive = new Hive
                    {
                        Name = $"Fake Hive {hiveCounter}",
                        ApiaryId = apiary.ApiaryId,
                        Measurements = new List<HiveMeasurement>()
                    };
                    hiveCounter++;
                    apiary.Hives.Add(hive);
                }
            }

            await _context.SaveChangesAsync();

            var startDate = DateTime.Now.AddDays(-14);
            var interval = TimeSpan.FromMinutes(15);
            int totalPoints = (int)(TimeSpan.FromDays(14).TotalMinutes / 15);

            var rnd = new Random();
            var hiveMeasurements = new List<HiveMeasurement>();
            var apiaryMeasurements = new List<ApiaryMeasurement>();

            foreach (var apiary in apiaries)
            {
                var time = startDate;
                for (int i = 0; i < totalPoints; i++)
                {
                    apiaryMeasurements.Add(new ApiaryMeasurement
                    {
                        ApiaryId = apiary.ApiaryId,
                        MeasurementDate = time,
                        Temperature = 25 + rnd.NextDouble() * 10 - 5,
                        //TODO: Pressure = 1010 + rnd.NextDouble() * 30 - 15,
                        LightIntensity = 300 + rnd.Next(-100, 200)
                    });
                    time = time.Add(interval);
                }

                foreach (var hive in apiary.Hives)
                {
                    time = startDate;
                    for (int i = 0; i < totalPoints; i++)
                    {
                        hiveMeasurements.Add(new HiveMeasurement
                        {
                            HiveId = hive.HiveId,
                            MeasurementDate = time,
                            Weight = 20 + rnd.NextDouble() * 5,
                            Temperature = 30 + rnd.NextDouble() * 10 - 5
                        });
                        time = time.Add(interval);
                    }
                }
            }

            _context.ApiaryMeasurements.AddRange(apiaryMeasurements);
            _context.HiveMeasurements.AddRange(hiveMeasurements);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Fake data seeding completed.");
        }
    }
}
