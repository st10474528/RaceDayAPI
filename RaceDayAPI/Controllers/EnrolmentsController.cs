using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    /// <summary>Enrols the logged-in participant into an event.</summary>
    [HttpPost("events/{eventId:int}/enrolments")]
    [AuthorizeRole("Participant")]
    public async Task<IActionResult> Enrol(int eventId, [FromBody] CreateEnrolmentDto dto)
    {
        var participantId = (int)HttpContext.Items["UserId"]!;

        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound(new { message = "Event not found." });

        var cat = await _db.Categories.FindAsync(dto.CategoryID);
        if (cat is null || cat.EventID != eventId) return BadRequest(new { message = "Invalid Category." });

        if (await _db.Enrolments.AnyAsync(e => e.ParticipantID == participantId && e.EventID == eventId))
            return Conflict(new { message = "Already enrolled." });

        var enrolment = new Enrolment { ParticipantID = participantId, EventID = eventId, CategoryID = dto.CategoryID, Status = "Confirmed" };
        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return StatusCode(201, new EnrolmentDto(enrolment.EnrolmentID, enrolment.ParticipantID, enrolment.EventID, enrolment.CategoryID, enrolment.EnrolmentDate, enrolment.Status));
    }

    /// <summary>Returns all events the logged-in participant has enrolled in.</summary>
    [HttpGet("enrolments/my")]
    [AuthorizeRole("Participant")]
    public async Task<IActionResult> MyEnrolments()
    {
        var participantId = (int)HttpContext.Items["UserId"]!;
        var list = await _db.Enrolments
            .Where(e => e.ParticipantID == participantId)
            .Select(e => new EnrolmentDto(e.EnrolmentID, e.ParticipantID, e.EventID, e.CategoryID, e.EnrolmentDate, e.Status))
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>Returns all participants enrolled in an organiser's event.</summary>
    [HttpGet("events/{eventId:int}/enrolments")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound();
        if (ev.OrganiserID != organiserId) return Forbid();

        var list = await _db.Enrolments
            .Where(e => e.EventID == eventId)
            .Select(e => new EnrolmentDto(e.EnrolmentID, e.ParticipantID, e.EventID, e.CategoryID, e.EnrolmentDate, e.Status))
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>Returns details of a specific enrolment.</summary>
    [HttpGet("enrolments/{id:int}")]
    [AuthorizeRole("Organiser", "Participant")]
    public async Task<IActionResult> GetById(int id)
    {
        var en = await _db.Enrolments.FindAsync(id);
        if (en is null) return NotFound();
        return Ok(new EnrolmentDto(en.EnrolmentID, en.ParticipantID, en.EventID, en.CategoryID, en.EnrolmentDate, en.Status));
    }

    /// <summary>Updates enrolment status.</summary>
    [HttpPut("enrolments/{id:int}/status")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var en = await _db.Enrolments.FindAsync(id);
        if (en is null) return NotFound();

        if (dto.Status != "Pending" && dto.Status != "Confirmed" && dto.Status != "Cancelled")
            return BadRequest(new { message = "Invalid status." });

        en.Status = dto.Status;
        await _db.SaveChangesAsync();
        return Ok(en);
    }

    /// <summary>Cancels an enrolment.</summary>
    [HttpDelete("enrolments/{id:int}")]
    [AuthorizeRole("Participant")]
    public async Task<IActionResult> Cancel(int id)
    {
        var participantId = (int)HttpContext.Items["UserId"]!;
        var en = await _db.Enrolments.FindAsync(id);
        if (en is null) return NotFound();
        if (en.ParticipantID != participantId) return Forbid();

        en.Status = "Cancelled";
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

// DTO needed for the UpdateStatus endpoint
public record UpdateStatusDto(string Status);