using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using BeeApp.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BeeApp.Web.Controllers
{
    public class ApiaryController : Controller
    {
        private readonly AppDbContext _context;

        public ApiaryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var apiaries = await _context.Apiaries
                .Include(a => a.Hives)
                .Include(a => a.Measurements)
                .ToListAsync();

            var vm = apiaries.Select(a => new ApiaryViewModel
            {
                ApiaryId = a.ApiaryId,
                Name = a.Name ?? string.Empty,
                HiveCount = a.Hives.Count,
                LastMeasurement = a.Measurements
                    .OrderByDescending(m => m.MeasurementDate)
                    .FirstOrDefault()?.MeasurementDate
            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiaryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var apiary = new Apiary
            {
                Name = dto.Name
            };

            _context.Apiaries.Add(apiary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var apiary = await _context.Apiaries.FindAsync(id);
            if (apiary == null) return NotFound();

            var dto = new ApiaryUpdateDto
            {
                ApiaryId = apiary.ApiaryId,
                Name = apiary.Name
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApiaryUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var apiary = await _context.Apiaries.FindAsync(dto.ApiaryId);
            if (apiary == null) return NotFound();

            apiary.Name = dto.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var apiary = await _context.Apiaries
                .Include(a => a.Hives)
                .FirstOrDefaultAsync(a => a.ApiaryId == id);

            if (apiary == null)
                return NotFound();

            return View(apiary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ApiaryId)
        {
            var apiary = await _context.Apiaries
                .Include(a => a.Hives)
                .FirstOrDefaultAsync(a => a.ApiaryId == ApiaryId);

            if (apiary == null)
                return NotFound();

            if (apiary.Hives.Any())
            {
                TempData["Error"] = "You cannot delete apiary which contains hives.";
                return RedirectToAction(nameof(Index));
            }

            _context.Apiaries.Remove(apiary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
