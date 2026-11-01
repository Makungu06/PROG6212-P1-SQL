using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2_RaceDay.API.Data;
using P2_RaceDay.API.Models;


namespace P2_RaceDay.API.Controllers;

public class EventDto
{
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[ApiController]
[Route("api/events")]


public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _context.Events.ToListAsync();
        return Ok(events);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvent(int id)
    {
        var raceEvent = await _context.Events
            .FindAsync(id);

        return raceEvent == null
            ? NotFound()
            : Ok(raceEvent);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateEvent(EventDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EventName) ||
            string.IsNullOrWhiteSpace(dto.Venue) ||
            string.IsNullOrWhiteSpace(dto.Location) ||
            string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest(
                "All event fields are required.");
        }

        if (dto.EventDate <= DateTime.UtcNow)
        {
            return BadRequest(
                "Event date must be in the future.");
        }

        var raceEvent = new RaceEvent
        {
            EventName = dto.EventName.Trim(),
            EventDate = dto.EventDate,
            Venue = dto.Venue.Trim(),
            Location = dto.Location.Trim(),
            Description = dto.Description.Trim()
        };

        _context.Events.Add(raceEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvent),
            new { id = raceEvent.EventID },
            raceEvent);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> UpdateEvent(
        int id, EventDto dto)
    {
        var raceEvent = await _context.Events
            .FindAsync(id);

        if (raceEvent == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.EventName) ||
            string.IsNullOrWhiteSpace(dto.Venue) ||
            string.IsNullOrWhiteSpace(dto.Location) ||
            string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest(
                "All event fields are required.");
        }

        raceEvent.EventName = dto.EventName.Trim();
        raceEvent.EventDate = dto.EventDate;
        raceEvent.Venue = dto.Venue.Trim();
        raceEvent.Location = dto.Location.Trim();
        raceEvent.Description = dto.Description.Trim();

        await _context.SaveChangesAsync();

        return Ok(raceEvent);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var raceEvent = await _context.Events
            .FindAsync(id);

        if (raceEvent == null)
            return NotFound();

        _context.Events.Remove(raceEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
