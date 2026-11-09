using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class OrganiserEventsController : Controller
{
    private readonly ApiClient _api;
    public OrganiserEventsController(ApiClient api) => _api = api;

    // GET: /OrganiserEvents/Create
    [HttpGet]
    public IActionResult Create() => View();

    // POST: /OrganiserEvents/Create
    [HttpPost]
    public async Task<IActionResult> Create(string name, string description, DateTime eventDate, string location, decimal distance, int eventTypeId, IFormFile banner)
    {
        var res = await _api.PostAsync("events", new
        {
            Name = name,
            Description = description,
            EventDate = eventDate,
            Location = location,
            Distance = distance,
            EventTypeID = eventTypeId
        });

        if (!res.IsSuccessStatusCode)
        {
            var errorDetails = await res.Content.ReadAsStringAsync();
            ViewBag.Error = $"API Error ({res.StatusCode}): {errorDetails}";
            return View();
        }

        var createdEvent = await _api.ReadAsync<EventDto>(res);

        if (banner != null && banner.Length > 0 && createdEvent != null)
        {
            await _api.PostFileAsync($"events/{createdEvent.EventID}/banner", banner);
        }

        return RedirectToAction("Index", "OrganiserDashboard");
    }

    // GET: /OrganiserEvents/Manage/5
    public async Task<IActionResult> Manage(int id)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Organiser") return RedirectToAction("Login", "Auth");

        var evRes = await _api.GetAsync($"events/{id}");
        if (!evRes.IsSuccessStatusCode) return NotFound();
        var ev = await _api.ReadAsync<EventDto>(evRes);

        var catRes = await _api.GetAsync($"events/{id}/categories");
        var categories = await _api.ReadAsync<List<CategoryDto>>(catRes) ?? new List<CategoryDto>();

        var enrRes = await _api.GetAsync($"events/{id}/enrolments");
        var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(enrRes) ?? new List<EnrolmentDto>();

        ViewBag.Categories = categories;
        ViewBag.Enrolments = enrolments;
        ViewBag.EventId = id;

        return View(ev);
    }

    // GET: /OrganiserEvents/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Organiser") return RedirectToAction("Login", "Auth");

        var res = await _api.GetAsync($"events/{id}");
        if (!res.IsSuccessStatusCode) return NotFound();

        var ev = await _api.ReadAsync<EventDto>(res);
        return View(ev);
    }

    // POST: /OrganiserEvents/Edit/5
    [HttpPost]
    public async Task<IActionResult> Edit(int id, string name, string description, DateTime eventDate, string location, decimal distance, int eventTypeId, IFormFile? banner)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Organiser") return RedirectToAction("Login", "Auth");

        // 1. Fetch current event to keep the existing banner URL if a new one isn't uploaded
        var currentRes = await _api.GetAsync($"events/{id}");
        var currentEvent = await _api.ReadAsync<EventDto>(currentRes);
        string? existingBannerUrl = currentEvent?.BannerImageUrl;

        // 2. Update event details (passing the existing banner URL so it doesn't get wiped out)
        var res = await _api.PutAsync($"events/{id}", new
        {
            EventTypeID = eventTypeId,
            Name = name,
            Description = description,
            EventDate = eventDate,
            Location = location,
            Distance = distance,
            BannerImageUrl = existingBannerUrl
        });

        if (!res.IsSuccessStatusCode)
        {
            var errorDetails = await res.Content.ReadAsStringAsync();
            ViewBag.Error = $"API Error ({res.StatusCode}): {errorDetails}";

            // Refetch the event to reload the page with correct values
            var ev = await _api.ReadAsync<EventDto>(res);
            return View(ev);
        }

        // 3. If a new banner was uploaded, send it to the API
        if (banner != null && banner.Length > 0)
        {
            await _api.PostFileAsync($"events/{id}/banner", banner);
        }

        TempData["Success"] = "Event updated successfully.";
        return RedirectToAction(nameof(Manage), new { id = id });
    }

    // POST: /OrganiserEvents/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Organiser") return RedirectToAction("Login", "Auth");

        var res = await _api.DeleteAsync($"events/{id}");

        if (res.IsSuccessStatusCode)
        {
            TempData["Success"] = "Event deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to delete event.";
        }

        return RedirectToAction("Index", "OrganiserDashboard");
    }

    // POST: /OrganiserEvents/AddCategory
    [HttpPost]
    public async Task<IActionResult> AddCategory(int eventId, string categoryName, int? minimumAge, int? maximumAge, decimal? distance)
    {
        await _api.PostAsync($"events/{eventId}/categories", new
        {
            CategoryName = categoryName,
            MinimumAge = minimumAge,
            MaximumAge = maximumAge,
            Distance = distance
        });

        return RedirectToAction(nameof(Manage), new { id = eventId });
    }

    // POST: /OrganiserEvents/CaptureResult
    [HttpPost]
    public async Task<IActionResult> CaptureResult(int enrolmentId, string finishTime, int finishingPosition, int eventId)
    {
        var res = await _api.PostAsync($"enrolments/{enrolmentId}/result", new
        {
            EnrolmentID = enrolmentId,
            FinishTime = finishTime,
            FinishingPosition = finishingPosition
        });

        if (!res.IsSuccessStatusCode)
        {
            TempData["Error"] = "Failed to capture result. Ensure time is in HH:mm:ss format.";
        }
        else
        {
            TempData["Success"] = "Result captured successfully!";
        }

        return RedirectToAction(nameof(Manage), new { id = eventId });
    }
}