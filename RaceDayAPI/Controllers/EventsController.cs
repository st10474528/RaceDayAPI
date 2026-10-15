using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventsController(RaceDayDbContext db) => _db = db;

    /// <summary>List all events. Public.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events
            .Select(e => new EventDto(e.EventID, e.OrganiserID, e.EventTypeID,
                e.Name, e.Description, e.EventDate, e.Location, e.Distance,
                e.BannerImageUrl, e.CreatedAt))
            .ToListAsync();
        return Ok(events);
    }

    /// <summary>Get a single event. Public.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _db.Events.FindAsync(id);
        if (e is null) return NotFound();
        return Ok(new EventDto(e.EventID, e.OrganiserID, e.EventTypeID,
            e.Name, e.Description, e.EventDate, e.Location, e.Distance,
            e.BannerImageUrl, e.CreatedAt));
    }

    /// <summary>Create a new event. Organisers only.</summary>
    [HttpPost]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;

        var ev = new Event
        {
            OrganiserID = organiserId,
            EventTypeID = dto.EventTypeID,
            Name = dto.Name,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Location = dto.Location,
            Distance = dto.Distance,
            BannerImageUrl = dto.BannerImageUrl
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = ev.EventID },
            new EventDto(ev.EventID, ev.OrganiserID, ev.EventTypeID, ev.Name,
                ev.Description, ev.EventDate, ev.Location, ev.Distance,
                ev.BannerImageUrl, ev.CreatedAt));
    }

    /// <summary>Update an event. Organisers only (must own the event).</summary>
    [HttpPut("{id:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserID != organiserId) return Forbid();

        ev.EventTypeID = dto.EventTypeID;
        ev.Name = dto.Name;
        ev.Description = dto.Description;
        ev.EventDate = dto.EventDate;
        ev.Location = dto.Location;
        ev.Distance = dto.Distance;
        ev.BannerImageUrl = dto.BannerImageUrl;

        await _db.SaveChangesAsync();
        return Ok(new EventDto(ev.EventID, ev.OrganiserID, ev.EventTypeID, ev.Name,
            ev.Description, ev.EventDate, ev.Location, ev.Distance,
            ev.BannerImageUrl, ev.CreatedAt));
    }

    /// <summary>Delete an event. Organisers only (must own the event).</summary>
    [HttpDelete("{id:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserID != organiserId) return Forbid();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}