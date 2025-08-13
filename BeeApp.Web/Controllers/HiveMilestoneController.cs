using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using BeeApp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Controllers
{
    public class HiveMilestoneController : Controller
    {
        private readonly AppDbContext _context;

        public HiveMilestoneController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int hiveId)
        {
            var hive = await _context.Hives
                .Include(h => h.Milestones)
                .FirstOrDefaultAsync(h => h.HiveId == hiveId);

            if (hive == null) return NotFound();

            ViewBag.HiveName = hive.Name;
            ViewBag.HiveId = hive.HiveId;

            var dtoList = hive.Milestones
                .OrderByDescending(m => m.Date)
                .Select(m => new HiveMilestoneDto
                {
                    HiveMilestoneId = m.HiveMilestoneId,
                    Date = m.Date,
                    Comment = m.Comment,
                    Type = m.Type
                }).ToList();

            return View(dtoList);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int hiveId, DateTime? date = null)
        {
            var selectedDate = date ?? DateTime.Today;

            var hive = await _context.Hives
                .Include(h => h.Measurements)
                .Include(h => h.Apiary)
                .FirstOrDefaultAsync(h => h.HiveId == hiveId);

            if (hive == null) return NotFound();

            var apiaryMeasurements = await _context.ApiaryMeasurements
                .Where(a => a.ApiaryId == hive.ApiaryId && a.MeasurementDate.Date == selectedDate.Date)
                .ToListAsync();

            var apiaryTempsByTime = apiaryMeasurements
                .GroupBy(a => a.MeasurementDate)
                .ToDictionary(g => g.Key, g => g.First().Temperature);

            var measurements = await _context.HiveMeasurements
                .Where(m => m.HiveId == hiveId && m.MeasurementDate.Date == selectedDate.Date)
                .OrderBy(m => m.MeasurementDate)
                .ToListAsync();

            var chartData = measurements
                .Select(m => new HiveMeasurementPoint
                {
                    Date = m.MeasurementDate,
                    Weight = Math.Round(m.Weight, 2),
                    Temperature = Math.Round(m.Temperature, 2),
                    ApiaryTemperature = apiaryTempsByTime.TryGetValue(m.MeasurementDate, out var t)
                        ? Math.Round(t, 2)
                        : (double?)null
                })
                .ToList();

            var milestones = await _context.HiveMilestones
                .Where(m => m.HiveId == hiveId && m.Date.Date == selectedDate.Date)
                .OrderBy(m => m.Date)
                .ToListAsync();

            ViewBag.Milestones = milestones;
            ViewBag.ChartData = chartData;
            ViewBag.HiveName = hive.Name;
            ViewBag.HiveId = hiveId;
            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");

            return View(new HiveMilestoneCreateDto
            {
                HiveId = hiveId,
                Date = selectedDate,
                Time = DateTime.Now.TimeOfDay
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HiveMilestoneCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var milestone = new HiveMilestone
            {
                HiveId = dto.HiveId,
                Date = dto.Date.Date + dto.Time,
                Comment = dto.Comment,
                Type = dto.Type
            };

            _context.HiveMilestones.Add(milestone);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { hiveId = dto.HiveId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var milestone = await _context.HiveMilestones.FindAsync(id);
            if (milestone == null) return NotFound();

            var dto = new HiveMilestoneUpdateDto
            {
                HiveMilestoneId = milestone.HiveMilestoneId,
                HiveId = milestone.HiveId,
                Date = milestone.Date.Date,
                Time = milestone.Date.TimeOfDay,
                Comment = milestone.Comment,
                Type = milestone.Type
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HiveMilestoneUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var milestone = await _context.HiveMilestones.FindAsync(dto.HiveMilestoneId);
            if (milestone == null) return NotFound();

            milestone.Date = dto.Date.Date + dto.Time;
            milestone.Comment = dto.Comment;
            milestone.Type = dto.Type;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { hiveId = milestone.HiveId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var milestone = await _context.HiveMilestones
                .Include(m => m.Hive)
                .FirstOrDefaultAsync(m => m.HiveMilestoneId == id);

            if (milestone == null) return NotFound();

            return View(milestone);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var milestone = await _context.HiveMilestones.FindAsync(id);
            if (milestone == null) return NotFound();

            int hiveId = milestone.HiveId;

            _context.HiveMilestones.Remove(milestone);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { hiveId });
        }

        public static string GetMilestoneIcon(MilestoneType type) =>
            type switch
            {
                MilestoneType.Feeding => "🍽️",
                MilestoneType.Harvesting => "🍯",
                MilestoneType.Inspection => "🔍",
                MilestoneType.QueenAdded => "👑",
                MilestoneType.Treatment => "💊",
                _ => "📍"
            };
    }
}
