using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class OrganiserDashboardController : Controller
{
    private readonly ApiClient _api;
    public OrganiserDashboardController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Organiser") return RedirectToAction("Login", "Auth");

        var res = await _api.GetAsync("events");
        var allEvents = await _api.ReadAsync<List<EventDto>>(res) ?? new List<EventDto>();

        // Filter to only this organiser's events
        var myEvents = allEvents.Where(e => e.OrganiserID == user.Value.UserId).ToList();
        return View(myEvents);
    }
}