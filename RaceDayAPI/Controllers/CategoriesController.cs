using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/events/{eventId:int}/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public CategoriesController(RaceDayDbContext db) => _db = db;

    /// <summary>List categories for an event. Public.</summary>
    [HttpGet]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        var categories = await _db.Categories
            .Where(c => c.EventID == eventId)
            .Select(c => new CategoryDto(c.CategoryID, c.EventID, c.CategoryName,
                c.MinimumAge, c.MaximumAge, c.Distance))
            .ToListAsync();
        return Ok(categories);
    }

    /// <summary>Add a category to an event. Organisers only.</summary>
    [HttpPost]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Create(int eventId, [FromBody] CreateCategoryDto dto)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound(new { message = "Event not found." });
        if (ev.OrganiserID != organiserId) return Forbid();

        var cat = new Category
        {
            EventID = eventId,
            CategoryName = dto.CategoryName,
            MinimumAge = dto.MinimumAge,
            MaximumAge = dto.MaximumAge,
            Distance = dto.Distance
        };

        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetForEvent), new { eventId },
            new CategoryDto(cat.CategoryID, cat.EventID, cat.CategoryName,
                cat.MinimumAge, cat.MaximumAge, cat.Distance));
    }

    /// <summary>Delete a category. Organisers only.</summary>
    [HttpDelete("{categoryId:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Delete(int eventId, int categoryId)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound();
        if (ev.OrganiserID != organiserId) return Forbid();

        var cat = await _db.Categories.FindAsync(categoryId);
        if (cat is null || cat.EventID != eventId) return NotFound();

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
