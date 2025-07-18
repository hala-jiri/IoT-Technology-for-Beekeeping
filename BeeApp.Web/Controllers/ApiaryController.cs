using BeeApp.Shared.Data;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;
using BeeApp.Shared.ViewModels;
using BeeApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace BeeApp.Web.Controllers
{
    public class ApiaryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public ApiaryController(AppDbContext context, IImageService imageService)
        {
            _imageService = imageService;
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
                ImageFileName = a.ImageFileName ?? string.Empty,
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
        public async Task<IActionResult> Create(ApiaryCreateDto dto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var apiary = new Apiary
            {
                Name = dto.Name,
                ImageFileName = dto.ImageFileName
            };


            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = await _imageService.SaveImageAsync(imageFile);
                apiary.ImageFileName = fileName;
            }

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
                Name = apiary.Name ?? string.Empty,
                Latitude = apiary.Latitude,
                Longitude = apiary.Longitude
                ImageFileName = apiary.ImageFileName,
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApiaryUpdateDto dto, IFormFile? imageFile, string? deleteImage)
        {
            if (!ModelState.IsValid) return View(dto);

            var apiary = await _context.Apiaries.FindAsync(dto.ApiaryId);
            if (apiary == null) return NotFound();

            apiary.Name = dto.Name;
            apiary.Longitude = dto.Longitude;
            apiary.Latitude = dto.Latitude;

            // Delete old pic
            if (!string.IsNullOrEmpty(deleteImage) && deleteImage == "true" && !string.IsNullOrEmpty(apiary.ImageFileName))
            {
                _imageService.DeleteImage(apiary.ImageFileName, "apiaries");
                apiary.ImageFileName = null;
            }

            // Save new pic
            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(apiary.ImageFileName))
                {
                    _imageService.DeleteImage(apiary.ImageFileName, "apiaries");
                }

                apiary.ImageFileName = await _imageService.SaveImageAsync(imageFile, "apiaries");
            }

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

            // Delete pic
            if (!string.IsNullOrEmpty(apiary.ImageFileName))
            {
                _imageService.DeleteImage(apiary.ImageFileName, "apiaries");
            }

            _context.Apiaries.Remove(apiary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
