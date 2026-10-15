using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    /// <summary>Enrol the current Participant in an event. Participants only.</summary>
    [HttpPost]
    [AuthorizeRole("Participant")]
    public async Task<IActionResult> Enrol([FromBody] CreateEnrolmentDto dto)
    {
        var participantId = (int)HttpContext.Items["UserId"]!;

        var ev = await _db.Events.FindAsync(dto.EventID);
        if (ev is null) return NotFound(new { message = "Event not found." });

        var cat = await _db.Categories.FindAsync(dto.CategoryID);
        if (cat is null || cat.EventID != dto.EventID)
            return BadRequest(new { message = "Category does not belong to this event." });

        if (await _db.Enrolments.AnyAsync(e =>
            e.ParticipantID == participantId && e.EventID == dto.EventID))
            return Conflict(new { message = "Already enrolled for this event." });

        var enrolment = new Enrolment
        {
            ParticipantID = participantId,
            EventID = dto.EventID,
            CategoryID = dto.CategoryID,
            Status = "Confirmed"
        };

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(MyEnrolments), null,
            new EnrolmentDto(enrolment.EnrolmentID, enrolment.ParticipantID,
                enrolment.EventID, enrolment.CategoryID,
                enrolment.EnrolmentDate, enrolment.Status));
    }

    /// <summary>List the current Participant's enrolments.</summary>
    [HttpGet("mine")]
    [AuthorizeRole("Participant")]
    public async Task<IActionResult> MyEnrolments()
    {
        var participantId = (int)HttpContext.Items["UserId"]!;
        var list = await _db.Enrolments
            .Where(e => e.ParticipantID == participantId)
            .Select(e => new EnrolmentDto(e.EnrolmentID, e.ParticipantID, e.EventID,
                e.CategoryID, e.EnrolmentDate, e.Status))
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>View all enrolments for one of the Organiser's events.</summary>
    [HttpGet("event/{eventId:int}")]
    [AuthorizeRole("Organiser")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var organiserId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound();
        if (ev.OrganiserID != organiserId) return Forbid();

        var list = await _db.Enrolments
            .Where(e => e.EventID == eventId)
            .Select(e => new EnrolmentDto(e.EnrolmentID, e.ParticipantID, e.EventID,
                e.CategoryID, e.EnrolmentDate, e.Status))
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>Cancel one of the Participant's own enrolments.</summary>
    [HttpDelete("{id:int}")]
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