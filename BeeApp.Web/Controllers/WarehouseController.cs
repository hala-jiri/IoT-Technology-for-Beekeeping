using BeeApp.Shared.Data;
using BeeApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeeApp.Web.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly AppDbContext _context;

        public WarehouseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.WarehouseItems
                .OrderBy(i => i.Status)
                .ThenBy(i => i.Name)
                .ToListAsync();

            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseItem item)
        {
            if (!ModelState.IsValid)
                return View(item);

            item.Created = DateTime.Now;
            _context.WarehouseItems.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.WarehouseItems.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WarehouseItem updated)
        {
            if (!ModelState.IsValid)
                return View(updated);

            var item = await _context.WarehouseItems.FindAsync(updated.WarehouseItemId);
            if (item == null) return NotFound();

            item.Name = updated.Name;
            item.Description = updated.Description;
            item.Quantity = updated.Quantity;
            item.Status = updated.Status;
            item.LocationNote = updated.LocationNote;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
