using BeeApp.Api.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeeApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeasurementController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] MeasurementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apiaryMeasurement = new ApiaryMeasurement
            {
                ApiaryId = dto.ApiaryId,
                MeasurementDate = dto.MeasurementDate ?? DateTime.UtcNow,
                Temperature = dto.Temperature,
                Humidity = dto.Humidity,
                LightIntensity = dto.LightIntensity
            };
            _context.ApiaryMeasurements.Add(apiaryMeasurement);

            foreach (var hive in dto.Hives)
            {
                var hiveMeasurement = new HiveMeasurement
                {
                    HiveId = hive.HiveId,
                    MeasurementDate = dto.MeasurementDate ?? DateTime.UtcNow,
                    Weight = hive.Weight,
                    Temperature = hive.Temperature,
                    Humidity = hive.Humidity
                };
                _context.HiveMeasurements.Add(hiveMeasurement);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
