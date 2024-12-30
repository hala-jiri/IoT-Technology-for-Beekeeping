using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ApiaryController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiaryController(AppDbContext context)
    {
        _context = context;
    }

    /*[HttpGet]
    public async Task<IActionResult> GetAllApiaries()
    {
       
    }*/

    /*[HttpPost]
    public async Task<IActionResult> AddApiary(Apiary apiary)
    {
        _context.Apiaries.Add(apiary);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAllApiaries), new { apiary.ApiaryNumber }, apiary);
    }*/
}