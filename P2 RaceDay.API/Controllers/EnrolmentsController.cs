using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2_RaceDay.API.Data;
using P2_RaceDay.API.Models;

namespace P2_RaceDay.API.Controllers;

public class EnrolmentDto
{
    public int RaceID { get; set; }
}

[ApiController]
[Route("api")]
[Authorize]

public class EnrolmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EnrolmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out userId);
    }

    // POST /api/events/{id}/enrolments
    [HttpPost("events/{id:int}/enrolments")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Enrol(
        int id, EnrolmentDto dto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var participant = await _context.Participants
            .FirstOrDefaultAsync(p => p.UserID == userId);

        if (participant == null)
            return BadRequest(
                "Create the participant profile before enrolling.");

        var race = await _context.Races
            .FirstOrDefaultAsync(r =>
                r.RaceID == dto.RaceID &&
                r.EventID == id);

        if (race == null)
            return BadRequest(
                "The selected race does not belong to this event.");

        var alreadyEnrolled = await _context.RaceEntries
            .AnyAsync(e =>
                e.RaceID == dto.RaceID &&
                e.ParticipantID == participant.ParticipantID &&
                e.Status != "Cancelled");

        if (alreadyEnrolled)
            return Conflict(
                "You are already enrolled in this race.");

        var entry = new RaceEntry
        {
            RaceID = dto.RaceID,
            ParticipantID = participant.ParticipantID,
            EntryDate = DateTime.UtcNow,
            Status = "Registered"
        };

        _context.RaceEntries.Add(entry);
        await _context.SaveChangesAsync();

        return StatusCode(201, new
        {
            message = "Enrolment successful.",
            entry.EntryID,
            entry.RaceID,
            entry.ParticipantID,
            entry.EntryDate,
            entry.Status
        });
    }

    // GET /api/enrolments/me
    [HttpGet("enrolments/me")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMyEnrolments()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var participant = await _context.Participants
            .FirstOrDefaultAsync(p => p.UserID == userId);

        if (participant == null)
            return Ok(Array.Empty<object>());

        var entries = await _context.RaceEntries
            .Where(e => e.ParticipantID ==
                participant.ParticipantID)
            .ToListAsync();

        return Ok(entries);
    }

    // GET /api/events/{id}/enrolments
    [HttpGet("events/{id:int}/enrolments")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetEventEnrolments(int id)
    {
        var eventExists = await _context.Events
            .AnyAsync(e => e.EventID == id);

        if (!eventExists)
            return NotFound("Event not found.");

        var entries = await _context.RaceEntries
            .Where(e => e.Race.EventID == id)
            .ToListAsync();

        return Ok(entries);
    }

    // DELETE /api/enrolments/{id}
    [HttpDelete("enrolments/{id:int}")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> CancelEnrolment(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var participant = await _context.Participants
            .FirstOrDefaultAsync(p => p.UserID == userId);

        if (participant == null)
            return NotFound("Participant profile not found.");

        var entry = await _context.RaceEntries
            .FirstOrDefaultAsync(e =>
                e.EntryID == id &&
                e.ParticipantID == participant.ParticipantID);

        if (entry == null)
            return NotFound("Enrolment not found.");

        if (entry.Status == "Cancelled")
            return Conflict("Enrolment is already cancelled.");

        entry.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
