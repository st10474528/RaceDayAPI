using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class EventsController : Controller
{
    private readonly ApiClient _api;
    public EventsController(ApiClient api) => _api = api;

    // GET: /Events
    public async Task<IActionResult> Index()
    {
        var res = await _api.GetAsync("events");
        var events = await _api.ReadAsync<List<EventDto>>(res) ?? new List<EventDto>();

        // Order by upcoming date
        events = events.OrderBy(e => e.EventDate).ToList();
        return View(events);
    }

    // GET: /Events/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var res = await _api.GetAsync($"events/{id}");
        if (!res.IsSuccessStatusCode) return NotFound();

        var ev = await _api.ReadAsync<EventDto>(res);
        if (ev is null) return NotFound();

        // Fetch categories for this event
        var catRes = await _api.GetAsync($"events/{id}/categories");
        var categories = await _api.ReadAsync<List<CategoryDto>>(catRes) ?? new List<CategoryDto>();

        ViewBag.Categories = categories;
        return View(ev);
    }
}