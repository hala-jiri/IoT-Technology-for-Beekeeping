using ApiaryDataCollector.Models;
using ApiaryDataCollector.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiaryDataCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiaryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        // Konstruktor controlleru s injekcí DbContextu a Mapperu
        public ApiaryController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // 1. Získání seznamu všech včelínů (zkrácené informace - ApiarySummaryDto)
        [HttpGet("summary")]
        public IActionResult GetApiarySummary()
        {
            var apiaries = _dbContext.Apiaries.ToList();
            var apiarySummaries = _mapper.Map<List<ApiarySummaryDto>>(apiaries);
            return Ok(apiarySummaries);
        }

        // 2. Získání detailního včelína podle ID
        [HttpGet("{id}")]
        public IActionResult GetApiaryById(int id)
        {
            var apiary = _dbContext.Apiaries.FirstOrDefault(a => a.ApiaryNumber == id);

            if (apiary == null)
            {
                return NotFound($"Apiary with ID {id} not found.");
            }

            var apiarySummary = _mapper.Map<ApiarySummaryDto>(apiary);
            return Ok(apiarySummary);
        }

        // 3. Přidání nového včelína
        [HttpPost]
        public IActionResult AddApiary([FromBody] ApiaryCreateDto apiaryCreateDto)
        {
            if (apiaryCreateDto == null)
            {
                return BadRequest("Invalid data.");
            }

            // Vytvoření nové entity Apiary z DTO
            var apiary = new Apiary
            {
                Name = apiaryCreateDto.Name
                // Další vlastnosti mohou být zde nastaveny podle potřeby
            };

            // Uložení do databáze
            _dbContext.Apiaries.Add(apiary);
            _dbContext.SaveChanges();

            // Mapování na ApiarySummaryDto a vrácení jako odpověď
            var apiarySummary = _mapper.Map<ApiarySummaryDto>(apiary);
            return CreatedAtAction(nameof(GetApiaryById), new { id = apiary.ApiaryNumber }, apiarySummary);
        }

        // 4. Aktualizace včelína
        [HttpPut("{id}")]
        public IActionResult UpdateApiary(int id, [FromBody] ApiaryUpdateDto apiaryUpdateDto)
        {
            if (apiaryUpdateDto == null)
            {
                return BadRequest("Invalid data.");
            }

            // Načteme existující včelín z databáze
            var apiary = _dbContext.Apiaries.FirstOrDefault(a => a.ApiaryNumber == id);

            if (apiary == null)
            {
                return NotFound($"Apiary with ID {id} not found.");
            }

            // Aktualizace dat
            apiary.Name = apiaryUpdateDto.Name;

            // Uložení změn do databáze
            _dbContext.SaveChanges();

            // Mapování na ApiarySummaryDto a vrácení jako odpověď
            var apiarySummary = _mapper.Map<ApiarySummaryDto>(apiary);
            return Ok(apiarySummary);
        }

        // 5. Smazání včelína
        [HttpDelete("{id}")]
        public IActionResult DeleteApiary(int id)
        {
            // Načteme včelín, který chceme smazat
            var apiary = _dbContext.Apiaries.FirstOrDefault(a => a.ApiaryNumber == id);

            if (apiary == null)
            {
                return NotFound($"Apiary with ID {id} not found.");
            }

            // Smazání včelína
            _dbContext.Apiaries.Remove(apiary);
            _dbContext.SaveChanges();

            return NoContent(); // Vrátíme status 204 No Content, protože data byla smazána
        }
    }
}