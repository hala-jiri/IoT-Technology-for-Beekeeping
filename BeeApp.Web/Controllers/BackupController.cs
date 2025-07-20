using BeeApp.Shared.Data;
using BeeApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Controllers
{
    public class BackupController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BackupService _backupService;
        private readonly IWebHostEnvironment _env;

        public BackupController(AppDbContext context, BackupService backupService, IWebHostEnvironment env)
        {
            _context = context;
            _backupService = backupService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var backups = await _context.Backups
                .OrderByDescending(b => b.Created)
                .Take(20)
                .ToListAsync();

            return View(backups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Run()
        {
            await _backupService.CreateBackupAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Download(string fileName)
        {
            var path = Path.Combine(_env.WebRootPath, "backups", fileName);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/zip", fileName);
        }
    }
}
