using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/event-types")]
public class EventTypesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventTypesController(RaceDayDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _db.EventTypes.ToListAsync();
        return Ok(types);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var type = await _db.EventTypes.FindAsync(id);
        if (type is null) return NotFound();
        return Ok(type);
    }
}