using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api")] // Changed to api so we can handle both /events/{id}/categories and /categories/{id}
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public CategoriesController(RaceDayDbContext db) => _db = db;

    /// <summary>Returns all categories for an event.</summary>
    [HttpGet("events/{eventId:int}/categories")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        var categories = await _db.Categories
            .Where(c => c.EventID == eventId)
            .Select(c => new CategoryDto(c.CategoryID, c.EventID, c.CategoryName, c.MinimumAge, c.MaximumAge, c.Distance))
            .ToListAsync();
        return Ok(categories);
    }

    /// <summary>Returns details of a specific category.</summary>
    [HttpGet("categories/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat is null) return NotFound();
        return Ok(new CategoryDto(cat.CategoryID, cat.EventID, cat.CategoryName, cat.MinimumAge, cat.MaximumAge, cat.Distance));
    }

    /// <summary>Creates a category for an event.</summary>
    [HttpPost("events/{eventId:int}/categories")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Create(int eventId, [FromBody] CreateCategoryDto dto)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound(new { message = "Event not found." });
        if (ev.OrganiserID != organiserId) return Forbid();

        var cat = new Category { EventID = eventId, CategoryName = dto.CategoryName, MinimumAge = dto.MinimumAge, MaximumAge = dto.MaximumAge, Distance = dto.Distance };
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();

        return StatusCode(201, new CategoryDto(cat.CategoryID, cat.EventID, cat.CategoryName, cat.MinimumAge, cat.MaximumAge, cat.Distance));
    }

    /// <summary>Updates a category.</summary>
    [HttpPut("categories/{id:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat is null) return NotFound();

        cat.CategoryName = dto.CategoryName;
        cat.MinimumAge = dto.MinimumAge;
        cat.MaximumAge = dto.MaximumAge;
        cat.Distance = dto.Distance;

        await _db.SaveChangesAsync();
        return Ok(cat);
    }

    /// <summary>Deletes a category.</summary>
    [HttpDelete("categories/{id:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat is null) return NotFound();

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}