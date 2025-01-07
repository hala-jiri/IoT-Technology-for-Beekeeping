using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

public class ApiaryController : Controller
{
    private readonly AppDbContext _context;

    public ApiaryController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var apiaries = await _context.Apiaries.Include(a=>a.Hives).ToListAsync();
        return View(apiaries);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Apiary apiary)
    {
        if (ModelState.IsValid)
        {
            _context.Apiaries.Add(apiary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(apiary);
    }
}