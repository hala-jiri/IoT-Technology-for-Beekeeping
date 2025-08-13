using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using BeeApp.Shared.ViewModels;
using BeeApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Controllers
{
    [Route("Feeding")]
    public class FeedingController : Controller
    {
        private readonly IFeedingService _service;
        private readonly AppDbContext _context;

        public FeedingController(IFeedingService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        // ===== Per-hive stránka (dříve FeedingController.Index) =====
        [HttpGet("Hive/{hiveId:int}")]
        public async Task<IActionResult> Hive(int hiveId, [FromQuery] int? seasonYear, [FromQuery] DateTime? date = null)
        {
            var year = seasonYear ?? DateTime.UtcNow.Year;
            var hive = await _context.Hives.AsNoTracking().FirstOrDefaultAsync(h => h.HiveId == hiveId);
            if (hive == null) return NotFound();

            var vm = new HiveFeedingIndexViewModel
            {
                HiveId = hiveId,
                HiveName = hive.Name,
                SeasonYear = year,
                Summary = await _service.GetHiveSummaryAsync(hiveId, year),
                Events = await _service.GetEventsAsync(hiveId, year),
                QuickAdd = new FeedingEventCreateDto
                {
                    HiveId = hiveId,
                    Date = date?.Date ?? DateTime.Today,
                    Medium = FeedingMedium.Syrup,
                    Unit = FeedingUnit.Liter
                }
            };

            return View("Hive", vm); // View: Views/Feeding/Hive.cshtml
        }

        [HttpPost("Hive/QuickAdd")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickAdd(FeedingEventCreateDto dto, [FromForm] int seasonYear)
        {
            if (dto.HiveId <= 0)
                ModelState.AddModelError("", "Chybí HiveId.");

            if (dto.Quantity <= 0)
                ModelState.AddModelError("Quantity", "Množství musí být > 0.");

            var hive = await _context.Hives
                .AsNoTracking()
                .Where(h => h.HiveId == dto.HiveId)
                .Select(h => new { h.HiveId, h.Name })
                .FirstOrDefaultAsync();

            if (hive == null)
                ModelState.AddModelError("", $"Úl #{dto.HiveId} nebyl nalezen.");

            if (!ModelState.IsValid)
            {
                var vm = new HiveFeedingIndexViewModel
                {
                    HiveId = dto.HiveId,
                    HiveName = hive?.Name ?? "(neznámý úl)",
                    SeasonYear = seasonYear,
                    Summary = dto.HiveId > 0 ? await _service.GetHiveSummaryAsync(dto.HiveId, seasonYear) : new HiveFeedingSummaryDto(),
                    Events = dto.HiveId > 0 ? await _service.GetEventsAsync(dto.HiveId, seasonYear) : new List<FeedingEvent>(),
                    QuickAdd = dto
                };
                return View("Hive", vm);
            }

            await _service.CreateAsync(dto);
            TempData["Msg"] = "Krmení přidáno.";
            return RedirectToAction(nameof(Hive), new { hiveId = dto.HiveId, seasonYear });
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromQuery] int seasonYear)
        {
            var e = await _context.FeedingEvents.FindAsync(id);
            if (e == null) return NotFound();

            var dto = new FeedingEventCreateDto
            {
                HiveId = e.HiveId,
                Date = e.Date,
                Medium = e.Medium,
                Quantity = e.Quantity,
                Unit = e.Unit,
                SyrupRatio = e.SyrupRatio,
                Additives = e.Additives,
                Note = e.Note,
                InspectionId = e.InspectionId
            };

            ViewBag.SeasonYear = seasonYear;
            ViewBag.HiveName = await _context.Hives.Where(h => h.HiveId == e.HiveId).Select(h => h.Name).FirstAsync();
            return View("Edit", dto); // View: Views/Feeding/Edit.cshtml
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FeedingEventCreateDto dto, [FromForm] int seasonYear)
        {
            await _service.UpdateAsync(id, dto);
            TempData["Msg"] = "Krmení upraveno.";
            return RedirectToAction(nameof(Hive), new { hiveId = dto.HiveId, seasonYear });
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int id, [FromForm] int hiveId, [FromForm] int seasonYear)
        {
            await _service.DeleteAsync(id);
            TempData["Msg"] = "Krmení smazáno.";
            return RedirectToAction(nameof(Hive), new { hiveId, seasonYear });
        }

        // ===== Dashboard (dříve FeedingDashboardController.Index) =====
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard([FromQuery] int? apiaryId, [FromQuery] int? seasonYear, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var year = seasonYear ?? DateTime.UtcNow.Year;

            var vm = new FeedingDashboardViewModel
            {
                ApiaryId = apiaryId,
                SeasonYear = year,
                From = from,
                To = to,
                Hives = await _service.GetDashboardAsync(apiaryId, year, from, to)
            };

            return View("Dashboard", vm); // View: Views/Feeding/Dashboard.cshtml
        }

        // GET: editor plánu pro úl/rok
        [HttpGet("Hive/{hiveId:int}/Plan")]
        public async Task<IActionResult> Plan(int hiveId, [FromQuery] int? seasonYear)
        {
            var year = seasonYear ?? DateTime.UtcNow.Year;
            var hive = await _context.Hives.AsNoTracking()
                .Where(h => h.HiveId == hiveId)
                .Select(h => new { h.HiveId, h.Name })
                .FirstOrDefaultAsync();
            if (hive == null) return NotFound();

            var plan = await _service.GetPlanAsync(hiveId, year);

            var vm = new FeedingPlanUpsertDto
            {
                HiveId = hiveId,
                SeasonYear = year,
                TargetSyrupLiters = plan?.TargetSyrupLiters,
                TargetPattyGrams = plan?.TargetPattyGrams,
                From = plan?.From.HasValue == true ? plan!.From.Value.ToDateTime(TimeOnly.MinValue) : null,
                To = plan?.To.HasValue == true ? plan!.To.Value.ToDateTime(TimeOnly.MinValue) : null
            };

            ViewBag.HiveName = hive.Name;
            return View("Plan", vm);
        }

        // POST: uložit (vytvořit/aktualizovat) plán
        [HttpPost("Hive/Plan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlanSave(FeedingPlanUpsertDto dto)
        {
            if (dto.HiveId <= 0) ModelState.AddModelError("", "Chybí HiveId.");
            if (dto.SeasonYear <= 0) ModelState.AddModelError("", "Chybí rok.");
            if (dto.TargetSyrupLiters < 0) ModelState.AddModelError(nameof(dto.TargetSyrupLiters), "Musí být ≥ 0.");
            if (dto.TargetPattyGrams < 0) ModelState.AddModelError(nameof(dto.TargetPattyGrams), "Musí být ≥ 0.");

            if (!ModelState.IsValid)
            {
                ViewBag.HiveName = await _context.Hives.Where(h => h.HiveId == dto.HiveId).Select(h => h.Name).FirstOrDefaultAsync() ?? "(neznámý úl)";
                return View("Plan", dto);
            }

            await _service.UpsertPlanAsync(dto);
            TempData["Msg"] = "Cíl krmení uložen.";
            return RedirectToAction(nameof(Hive), new { hiveId = dto.HiveId, seasonYear = dto.SeasonYear });
        }
    }
}
