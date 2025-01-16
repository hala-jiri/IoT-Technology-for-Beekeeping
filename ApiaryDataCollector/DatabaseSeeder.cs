using ApiaryDataCollector.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;

    public DatabaseSeeder(AppDbContext context)
    {
        _context = context;
    }

    public void SeedDatabase()
    {
        // Clear the database
        _context.HiveMeasurement.ExecuteDelete();
        _context.ApiaryMeasurement.ExecuteDelete();
        _context.Hives.ExecuteDelete();
        _context.Apiaries.ExecuteDelete();

        _context.SaveChanges();

        // Create Apiaries
        var apiaries = new List<Apiary>
        {
            new Apiary { Name = "Apiary Alpha" },
            new Apiary { Name = "Apiary Beta" },
            new Apiary { Name = "Apiary Gamma" },
            new Apiary { Name = "Apiary Delta" }
        };

        _context.Apiaries.AddRange(apiaries);
        _context.SaveChanges();

        // Create Hives
        var random = new Random();
        foreach (var apiary in apiaries)
        {
            // Create Hives for each Apiary
            int hivesPerApiary = random.Next(4, 8); // Randomly assign 4-8 hives per Apiary
            var hives = new List<Hive>();

            for (int i = 0; i < hivesPerApiary; i++)
            {
                var hive = new Hive
                {
                    ApiaryNumber = apiary.ApiaryNumber,
                    Name = $"Hive {_context.Hives.Count() + 1}",
                    Apiary = apiary,
                };
                _context.Hives.Add(hive);
                _context.SaveChanges();
            }
        }

        // Add measurements for apiary and hives under apiary
        foreach (var apiary in apiaries)
        {
            var apiaryMeasurementsList = new List<ApiaryMeasurement>();
            var hiveMeasurementsList = new List<HiveMeasurement>();
            DateTime startDate = DateTime.Now.AddDays(-30); // Start from 30 days ago

            foreach (var hiveInApiary in apiary.Hives)
            {
                AddHiveMeasurements(hiveInApiary, startDate, 5000);
            }
            AddApiaryMeasurements(apiary, startDate, 5000);
        }
    }
        

    // Save changes to database
    //_context.SaveChanges();

    private void AddApiaryMeasurements(Apiary apiary, DateTime startDate, int measurements)
    {
        var apiaryMeasurements = new List<ApiaryMeasurement>();
        Random randomer = new Random();
        var currentLightIntensity = randomer.Next(200, 800);
        var currentTemperature = Math.Round((double)randomer.Next(5, 35), 2);
        var currentHumidity = randomer.Next(40, 80);
        for (int i = 1; i <= measurements; i++)
        {
            var measurement = new ApiaryMeasurement() 
            {
                Apiary = apiary,
                ApiaryNumber = apiary.ApiaryNumber,
                Humidity = currentHumidity,
                LightIntensity = currentLightIntensity,
                Temperature = currentTemperature,
                MeasurementDate = startDate.AddMinutes(i * 15)
            };
            apiaryMeasurements.Add(measurement);

            // adjust next humidity
            currentHumidity = RandomHumidity(currentHumidity);
            // adjust next temperature 
            currentTemperature = RandomTemperature(currentTemperature);
            // adjust next LightIntensity 
            currentLightIntensity = RandomLightIntensity(currentLightIntensity);
        }
        _context.ApiaryMeasurement.AddRange(apiaryMeasurements);
        _context.SaveChanges();
    }

    private void AddHiveMeasurements(Hive hive, DateTime startDate, int measurements)
    {
        var hiveMeasurements = new List<HiveMeasurement>();
        Random randomer = new Random();
        var currentWeight = Math.Round((double)randomer.Next(15, 45), 2);
        var currentTemperature = Math.Round((double)randomer.Next(32, 35), 2);
        var currentHumidity = randomer.Next(50, 70);

        for (int i = 1; i <= measurements; i++)
        {
            var measurement = new HiveMeasurement()
            {
                Hive = hive,
                HiveNumber = hive.HiveNumber,
                MeasurementDate = startDate.AddMinutes(i * 15),
                Weight = currentWeight,
                Temperature = currentTemperature,
                Humidity = currentHumidity
            };
            hiveMeasurements.Add(measurement);

            // adjust next humidity
            currentHumidity = RandomHumidity(currentHumidity);
            // adjust next temperature 
            currentTemperature = RandomTemperature(currentTemperature);
            // adjust next weight 
            currentWeight = RandomWeight(currentWeight);
        }
        _context.HiveMeasurement.AddRange(hiveMeasurements);
        _context.SaveChanges();
    }

    private int RandomHumidity(int startHumidity)
    {
        // regular range is (50 - 70)
        Random random = new Random();
        if (startHumidity > 80)
        {
            return (startHumidity - random.Next(0,3));
        }
        else if (startHumidity < 40)
        {
            return (startHumidity + random.Next(0, 3));
        }
        else
        {
            return (startHumidity + random.Next(-3,3));
        }
    }

    private double RandomTemperature(double startTemperature)
    {
        // regular range is 30 - 35
        Random random = new Random();
        if (startTemperature > 35)
        {
            return Math.Round(startTemperature - random.NextDouble() * 2, 2);
        }
        else if (startTemperature < 30)
        {
            return Math.Round(startTemperature + random.NextDouble() * 2, 2);
        }
        else
        {
            return Math.Round(startTemperature + (random.NextDouble() * 4) - 2, 2);
        }
    }

    private double RandomWeight(double startWeight)
    {
        Random random = new Random();
        return Math.Round(startWeight + (random.NextDouble() - 0.5),3);
    }

    private int RandomLightIntensity(int startIntensity)
    {
        Random random = new Random();
        return startIntensity + random.Next(-10,10);
    }

    private void ClearDatabase()
    {
        _context.Database.ExecuteSqlRaw("DELETE FROM HiveMeasurements");
        _context.Database.ExecuteSqlRaw("DELETE FROM ApiaryMeasurements");
        _context.Database.ExecuteSqlRaw("DELETE FROM Hives");
        _context.Database.ExecuteSqlRaw("DELETE FROM Apiaries");
        _context.SaveChanges();
    }
}