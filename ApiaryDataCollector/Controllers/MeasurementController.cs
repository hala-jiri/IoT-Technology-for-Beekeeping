using ApiaryDataCollector.Models;
using ApiaryDataCollector.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiaryDataCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        // Konstruktor controlleru s injekcí DbContextu a Mapperu
        public MeasurementController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // POST: api/Measurement
        [HttpPost]
        public IActionResult AddMeasurements([FromBody] ApiaryDto apiaryDto)
        {
            if (apiaryDto == null)
            {
                return BadRequest("Invalid data.");
            }

            // 1. Zpracujeme ApiaryMeasurement
            var apiary = _dbContext.Apiaries
                .FirstOrDefault(a => a.ApiaryNumber == apiaryDto.ApiaryNumber);

            if (apiary == null)
            {
                return NotFound($"Apiary with number {apiaryDto.ApiaryNumber} not found.");
            }

            // Vytvoříme ApiaryMeasurement z JSON dat
            var apiaryMeasurement = new ApiaryMeasurement
            {
                ApiaryNumber = apiaryDto.ApiaryNumber,
                Apiary = apiary, // Pokud Apiary existuje
                MeasurementDate = apiaryDto.ReportDate, // Používáme ReportDate z ApiaryDto pro všechna měření
                Humidity = apiaryDto.Humidity,
                Temperature = apiaryDto.Temperature,
                LightIntensity = apiaryDto.LightIntensity
            };

            // Přidáme měření do seznamu měření Apiary
            apiary.Measurements.Add(apiaryMeasurement);

            // 2. Zpracujeme HiveMeasurement pro každý úl
            if (apiaryDto.Hives != null)  // Zajistíme, že Hives není null
            {
                foreach (var hiveDto in apiaryDto.Hives)
                {
                    var hive = _dbContext.Hives
                        .FirstOrDefault(h => h.HiveNumber == hiveDto.HiveNumber && h.ApiaryNumber == apiaryDto.ApiaryNumber);

                    if (hive == null)
                    {
                        return NotFound($"Hive with number {hiveDto.HiveNumber} not found in Apiary {apiaryDto.ApiaryNumber}.");
                    }

                    // Vytvoříme HiveMeasurement
                    var hiveMeasurement = new HiveMeasurement
                    {
                        HiveNumber = hiveDto.HiveNumber,
                        Hive = hive, // Pokud Hive existuje
                        MeasurementDate = apiaryDto.ReportDate, // Používáme ReportDate pro všechna měření Hive
                        Weight = hiveDto.Weight,
                        Temperature = hiveDto.Temperature,
                        Humidity = hiveDto.Humidity
                    };

                    // Přidáme měření do seznamu měření pro tento Hive
                    hive.Measurements.Add(hiveMeasurement);
                }
            }
            // Uložení všech změn do databáze
            _dbContext.SaveChanges();

            return Ok("Measurements added successfully.");
        }
    }
}