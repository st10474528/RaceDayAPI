using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;

    /// <summary>Capture a result for an enrolment. Organisers only.</summary>
    [HttpPost]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Create([FromBody] CreateResultDto dto)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var enrolment = await _db.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentID == dto.EnrolmentID);
        if (enrolment is null) return NotFound(new { message = "Enrolment not found." });
        if (enrolment.Event!.OrganiserID != organiserId) return Forbid();

        if (!TimeSpan.TryParse(dto.FinishTime, out var finish))
            return BadRequest(new { message = "FinishTime must be HH:mm:ss." });
        if (dto.FinishingPosition <= 0)
            return BadRequest(new { message = "FinishingPosition must be > 0." });

        if (await _db.Results.AnyAsync(r => r.EnrolmentID == dto.EnrolmentID))
            return Conflict(new { message = "Result already captured." });

        var result = new Result
        {
            EnrolmentID = dto.EnrolmentID,
            FinishTime = finish,
            FinishingPosition = dto.FinishingPosition
        };

        _db.Results.Add(result);
        await _db.SaveChangesAsync();

        return Ok(new ResultDto(result.ResultID, result.EnrolmentID,
            result.FinishTime.ToString(), result.FinishingPosition, result.RecordedAt));
    }

    /// <summary>List the current Participant's own results.</summary>
    [HttpGet("mine")]
    [AuthorizeRole("Participant")]
    public async Task<IActionResult> Mine()
    {
        var participantId = (int)HttpContext.Items["UserId"]!;
        var results = await _db.Results
            .Include(r => r.Enrolment)
            .Where(r => r.Enrolment!.ParticipantID == participantId)
            .Select(r => new ResultDto(r.ResultID, r.EnrolmentID,
                r.FinishTime.ToString(), r.FinishingPosition, r.RecordedAt))
            .ToListAsync();
        return Ok(results);
    }

    /// <summary>List all results for one of the Organiser's events.</summary>
    [HttpGet("event/{eventId:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound();
        if (ev.OrganiserID != organiserId) return Forbid();

        var results = await _db.Results
            .Include(r => r.Enrolment)
            .Where(r => r.Enrolment!.EventID == eventId)
            .Select(r => new ResultDto(r.ResultID, r.EnrolmentID,
                r.FinishTime.ToString(), r.FinishingPosition, r.RecordedAt))
            .ToListAsync();
        return Ok(results);
    }
}
