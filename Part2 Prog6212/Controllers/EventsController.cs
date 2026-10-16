using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Part2_Prog6212.Data;
using Part2_Prog6212.Models;
using Part2_Prog6212.DTOs;
namespace Part2_Prog6212.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EventsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events
            .AsNoTracking()
            .Include(e => e.Races)
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.Events
            .AsNoTracking()
            .Include(e => e.Races)
            .FirstOrDefaultAsync(e => e.RaceEventId == id);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(EventRequest request)
    {
        var item = new RaceEvent
        {
            EventName = request.EventName,
            EventDate = request.EventDate,
            Venue = request.Venue,
            Location = request.Location,
            Description = request.Description,
            EventType = request.EventType,
            Capacity = request.Capacity
        };

        _db.Events.Add(item);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.RaceEventId },
            item);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(
        int id, EventRequest request)
    {
        var item = await _db.Events.FindAsync(id);

        if (item == null)
            return NotFound();

        item.EventName = request.EventName;
        item.EventDate = request.EventDate;
        item.Venue = request.Venue;
        item.Location = request.Location;
        item.Description = request.Description;
        item.EventType = request.EventType;
        item.Capacity = request.Capacity;

        await _db.SaveChangesAsync();

        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Events.FindAsync(id);

        if (item == null)
            return NotFound();

        _db.Events.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

