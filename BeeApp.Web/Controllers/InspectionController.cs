using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using BeeApp.Shared.ViewModels;
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
        public async Task<IActionResult> Index(int hiveId)
        {
            var hive = await _context.Hives
                .Include(h => h.Apiary)
                .Include(h => h.InspectionReports)
                .FirstOrDefaultAsync(h => h.HiveId == hiveId);

            if (hive == null) return NotFound();

            var vm = new HiveInspectionListViewModel
            {
                HiveId = hive.HiveId,
                HiveName = hive.Name,
                ApiaryName = hive.Apiary?.Name ?? "(Unknown)",
                Inspections = hive.InspectionReports.OrderByDescending(i => i.InspectionDate).ToList()
            };

            return View(vm);
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var report = await _context.InspectionReports
                .Include(i => i.Hive)
                .ThenInclude(h => h.Apiary)
                .FirstOrDefaultAsync(i => i.InspectionReportId == id);

            if (report == null) return NotFound();

            ViewBag.HiveName = report.Hive?.Name;
            ViewBag.ApiaryName = report.Hive?.Apiary?.Name;

            var dto = new InspectionEditDto
            {
                InspectionReportId = report.InspectionReportId,
                HiveId = report.HiveId,
                InspectionDate = report.InspectionDate,
                QueenSeen = report.QueenSeen,
                BroodPresent = report.BroodPresent,
                EggsPresent = report.EggsPresent,
                PollenPresent = report.PollenPresent,
                HoneyPresent = report.HoneyPresent,
                Notes = report.Notes
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InspectionEditDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var report = await _context.InspectionReports.FindAsync(dto.InspectionReportId);
            if (report == null) return NotFound();

            report.InspectionDate = dto.InspectionDate;
            report.QueenSeen = dto.QueenSeen;
            report.BroodPresent = dto.BroodPresent;
            report.EggsPresent = dto.EggsPresent;
            report.PollenPresent = dto.PollenPresent;
            report.HoneyPresent = dto.HoneyPresent;
            report.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { hiveId = dto.HiveId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var inspection = await _context.InspectionReports
                .Include(i => i.Hive)
                .FirstOrDefaultAsync(i => i.InspectionReportId == id);

            if (inspection == null) return NotFound();

            ViewBag.HiveName = inspection.Hive?.Name;
            return View(inspection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.InspectionReports.FindAsync(id);
            if (inspection == null) return NotFound();

            int hiveId = inspection.HiveId;

            _context.InspectionReports.Remove(inspection);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { hiveId });
        }
    }
}
