using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class EnrolmentsController : Controller
{
    private readonly ApiClient _api;
    public EnrolmentsController(ApiClient api) => _api = api;

    [HttpPost]
    public async Task<IActionResult> Enrol(int EventID, int CategoryID)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Participant") return RedirectToAction("Login", "Auth");

        // 👇 Updated to match plan: POST /api/events/{eventId}/enrolments
        var res = await _api.PostAsync($"events/{EventID}/enrolments", new { EventID, CategoryID });

        if (!res.IsSuccessStatusCode)
        {
            TempData["Error"] = "Failed to enrol. You may already be enrolled.";
            return RedirectToAction("Details", "Events", new { id = EventID });
        }

        TempData["Success"] = "Successfully enrolled!";
        return RedirectToAction(nameof(Mine));
    }

    public async Task<IActionResult> Mine()
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Participant") return RedirectToAction("Login", "Auth");

        // 👇 Updated to match plan: GET /api/enrolments/my
        var res = await _api.GetAsync("enrolments/my");
        var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(res) ?? new List<EnrolmentDto>();

        return View(enrolments);
    }
}