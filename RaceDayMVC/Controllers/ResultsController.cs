using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class ResultsController : Controller
{
    private readonly ApiClient _api;
    public ResultsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Mine()
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Participant") return RedirectToAction("Login", "Auth");

        // 👇 Updated to match plan: GET /api/results/my
        var res = await _api.GetAsync("results/my");
        var results = await _api.ReadAsync<List<ResultDto>>(res) ?? new List<ResultDto>();

        return View(results);
    }
}