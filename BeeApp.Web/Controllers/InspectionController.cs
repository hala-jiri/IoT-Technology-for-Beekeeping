using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Controllers
{
    public class InspectionController : Controller
    {
        private readonly AppDbContext _context;

        public InspectionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int hiveId)
        {
            var hive = await _context.Hives.Include(h => h.Apiary).FirstOrDefaultAsync(h => h.HiveId == hiveId);
            if (hive == null) return NotFound();

            ViewBag.HiveName = hive.Name;
            ViewBag.ApiaryName = hive.Apiary?.Name;

            var dto = new InspectionCreateDto
            {
                HiveId = hiveId
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InspectionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var entity = new InspectionReport
            {
                HiveId = dto.HiveId,
                InspectionDate = dto.InspectionDate,
                QueenSeen = dto.QueenSeen,
                BroodPresent = dto.BroodPresent,
                EggsPresent = dto.EggsPresent,
                PollenPresent = dto.PollenPresent,
                HoneyPresent = dto.HoneyPresent,
                Notes = dto.Notes
            };

            _context.InspectionReports.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Hive", new { id = dto.HiveId });
        }
    }
}
