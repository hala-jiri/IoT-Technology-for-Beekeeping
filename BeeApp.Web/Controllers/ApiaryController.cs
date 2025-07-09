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
    }
}
