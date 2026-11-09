using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;

    /// <summary>Records the participant's finishing time and position.</summary>
    [HttpPost("enrolments/{enrolmentId:int}/result")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Create(int enrolmentId, [FromBody] CreateResultDto dto)
    {
        var enrolment = await _db.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.EnrolmentID == enrolmentId);
        if (enrolment is null) return NotFound();

        if (!TimeSpan.TryParse(dto.FinishTime, out var finish))
            return BadRequest(new { message = "FinishTime must be HH:mm:ss." });

        var result = new Result { EnrolmentID = enrolmentId, FinishTime = finish, FinishingPosition = dto.FinishingPosition };
        _db.Results.Add(result);
        await _db.SaveChangesAsync();

        return StatusCode(201, new ResultDto(result.ResultID, result.EnrolmentID, result.FinishTime.ToString(), result.FinishingPosition, result.RecordedAt));
    }

    /// <summary>Updates a recorded result.</summary>
    [HttpPut("results/{id:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateResultDto dto)
    {
        var result = await _db.Results.FindAsync(id);
        if (result is null) return NotFound();

        if (!TimeSpan.TryParse(dto.FinishTime, out var finish))
            return BadRequest(new { message = "FinishTime must be HH:mm:ss." });

        result.FinishTime = finish;
        result.FinishingPosition = dto.FinishingPosition;
        await _db.SaveChangesAsync();

        return Ok(new ResultDto(result.ResultID, result.EnrolmentID, result.FinishTime.ToString(), result.FinishingPosition, result.RecordedAt));
    }

    /// <summary>Returns the logged-in participant's race history.</summary>
    [HttpGet("results/my")]
    [AuthorizeRole("Participant")]
    public async Task<IActionResult> Mine()
    {
        var participantId = (int)HttpContext.Items["UserId"]!;
        var results = await _db.Results
            .Include(r => r.Enrolment)
            .Where(r => r.Enrolment!.ParticipantID == participantId)
            .Select(r => new ResultDto(r.ResultID, r.EnrolmentID, r.FinishTime.ToString(), r.FinishingPosition, r.RecordedAt))
            .ToListAsync();
        return Ok(results);
    }

    /// <summary>Returns results for an event.</summary>
    [HttpGet("events/{eventId:int}/results")]
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
            .Select(r => new ResultDto(r.ResultID, r.EnrolmentID, r.FinishTime.ToString(), r.FinishingPosition, r.RecordedAt))
            .ToListAsync();
        return Ok(results);
    }

    /// <summary>Returns details of a specific result.</summary>
    [HttpGet("results/{id:int}")]
    [AuthorizeRole("Organiser", "Participant")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _db.Results.FindAsync(id);
        if (r is null) return NotFound();
        return Ok(new ResultDto(r.ResultID, r.EnrolmentID, r.FinishTime.ToString(), r.FinishingPosition, r.RecordedAt));
    }
}