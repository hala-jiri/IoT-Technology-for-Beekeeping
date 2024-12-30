using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiaryController : Controller
{
    private readonly HttpClient _httpClient;

    public ApiaryController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetStringAsync("https://localhost:5001/api/Apiary");
        var apiaries = JsonConvert.DeserializeObject<List<Apiary>>(response);

        return View(apiaries);
    }
}