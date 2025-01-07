using ApiaryDataCollector.Models;
using ApiaryDataCollector.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiaryDataCollector.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HiveController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        // Konstruktor kontroleru s injekcí DbContextu a Mapperu
        public HiveController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // POST: api/Hive
        [HttpPost]
        public async Task<IActionResult> AddHive([FromBody] HiveCreateDto hiveCreateDto)
        {
            if (hiveCreateDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var apiary = await _dbContext.Apiaries
                .FirstOrDefaultAsync(a => a.ApiaryNumber == hiveCreateDto.ApiaryNumber);

            if (apiary == null)
            {
                return NotFound($"Apiary with number {hiveCreateDto.ApiaryNumber} not found.");
            }

            var hive = _mapper.Map<Hive>(hiveCreateDto);
            hive.Apiary = apiary; // Přiřazení Apiary

            _dbContext.Hives.Add(hive);
            await _dbContext.SaveChangesAsync();

            var hiveSummaryDto = new HiveSummaryDto
            {
                HiveNumber = hive.HiveNumber,
                Name = hive.Name,
                ApiaryNumber = hive.ApiaryNumber,
                ApiaryName = hive.Apiary.Name != null ? hive.Apiary.Name : "Unknown"
            };

            return CreatedAtAction(nameof(GetHive), new { id = hiveSummaryDto.HiveNumber }, hiveSummaryDto);
        }

        // PUT: api/Hive/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHive(int id, [FromBody] HiveUpdateDto hiveUpdateDto)
        {
            if (hiveUpdateDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var hive = await _dbContext.Hives
                .FirstOrDefaultAsync(h => h.HiveNumber == id);

            if (hive == null)
            {
                return NotFound($"Hive with number {id} not found.");
            }

            _mapper.Map(hiveUpdateDto, hive);

            _dbContext.Hives.Update(hive);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Hive/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HiveSummaryDto>> GetHive(int id)
        {
            var hive = await _dbContext.Hives
                .FirstOrDefaultAsync(h => h.HiveNumber == id);

            if (hive == null)
            {
                return NotFound($"Hive with number {id} not found.");
            }

            var hiveSummaryDto = _mapper.Map<HiveSummaryDto>(hive);
            return Ok(hiveSummaryDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHive(int id)
        {
            // Najdi úl podle HiveNumber
            var hive = await _dbContext.Hives
                .FirstOrDefaultAsync(h => h.HiveNumber == id);

            if (hive == null)
            {
                return NotFound($"Hive with number {id} not found.");
            }

            // Smazání úlu
            _dbContext.Hives.Remove(hive);
            await _dbContext.SaveChangesAsync();

            return NoContent(); // Vrací HTTP 204 No Content
        }

        // GET: api/Hive/summary
        [HttpGet("summary")]
        public async Task<ActionResult<List<HiveSummaryDto>>> GetHiveSummary()
        {
            var hives = await _dbContext.Hives
                .Include(h => h.Apiary) // Include Apiary to get Apiary info
                .Include(h => h.Measurements) // Include Measurements for Hive
                .ToListAsync();

            if (hives == null || hives.Count == 0)
            {
                return NotFound("No hives found.");
            }

            var hiveSummaries = _mapper.Map<List<HiveSummaryDto>>(hives);
            return Ok(hiveSummaries);
        }
    }
}