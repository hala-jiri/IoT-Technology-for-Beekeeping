using BeeApp.Shared.Data;
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
            if (dto == null)
                return BadRequest("Invalid payload");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apiaryMeasurement = new ApiaryMeasurement
            {
                ApiaryId = dto.ApiaryId,
                MeasurementDate = dto.MeasurementDate ?? DateTime.UtcNow,
                Temperature = dto.Temperature ?? 0,
                Humidity = dto.Humidity ?? 0,
                LightIntensity = dto.LightIntensity ?? 0
            };
            _context.ApiaryMeasurements.Add(apiaryMeasurement);

            foreach (var hive in dto.Hives)
            {
                var hiveMeasurement = new HiveMeasurement
                {
                    HiveId = hive.HiveId,
                    MeasurementDate = dto.MeasurementDate ?? DateTime.UtcNow,
                    Weight = hive.Weight ?? 0,
                    Temperature = hive.Temperature ?? 0,
                    Humidity = hive.Humidity ?? 0
                };
                _context.HiveMeasurements.Add(hiveMeasurement);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
