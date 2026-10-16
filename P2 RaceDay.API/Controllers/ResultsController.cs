using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2_RaceDay.API.Data;
using P2_RaceDay.API.Models;

namespace P2_RaceDay.API.Controllers;

public class ResultDto
{
    public int EntryID { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public string RaceStatus { get; set; } = "Finished";
}

public class UpdateResultDto
{
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public string RaceStatus { get; set; } = "Finished";
}

[ApiController]
[Route("api/results")]

public class ResultsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ResultsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetResults()
    {
        return Ok(await _context.Results.ToListAsync());
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetResult(int id)
    {
        var result = await _context.Results.FindAsync(id);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("/api/events/{id:int}/results")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEventResults(int id)
    {
        var eventExists = await _context.Events
            .AnyAsync(e => e.EventID == id);

        if (!eventExists)
            return NotFound("Event not found.");

        var results = await _context.Results
            .Where(r => r.Entry.Race.EventID == id)
            .ToListAsync();

        return Ok(results);
    }

    [HttpPost("/api/events/{id:int}/results")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateResult(
        int id, ResultDto dto)
    {
        var entry = await _context.RaceEntries
            .Include(e => e.Race)
            .FirstOrDefaultAsync(e => e.EntryID == dto.EntryID);

        if (entry == null || entry.Race.EventID != id)
            return BadRequest(
                "Entry does not belong to this event.");

        if (await _context.Results.AnyAsync(
            r => r.EntryID == dto.EntryID))
        {
            return Conflict(
                "A result already exists for this entry.");
        }

        if (dto.Position.HasValue && dto.Position.Value < 1)
            return BadRequest("Position must be at least 1.");

        if (dto.FinishTime.HasValue &&
            dto.FinishTime.Value < TimeSpan.Zero)
            return BadRequest("Finish time cannot be negative.");

        var result = new RaceResult
        {
            EntryID = dto.EntryID,
            FinishTime = dto.FinishTime,
            Position = dto.Position,
            RaceStatus = dto.RaceStatus.Trim()
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return StatusCode(201, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> UpdateResult(
        int id, UpdateResultDto dto)
    {
        var result = await _context.Results.FindAsync(id);

        if (result == null)
            return NotFound();

        if (dto.Position.HasValue && dto.Position.Value < 1)
            return BadRequest("Position must be at least 1.");

        if (dto.FinishTime.HasValue &&
            dto.FinishTime.Value < TimeSpan.Zero)
            return BadRequest("Finish time cannot be negative.");

        if (string.IsNullOrWhiteSpace(dto.RaceStatus))
            return BadRequest("Race status is required.");

        result.FinishTime = dto.FinishTime;
        result.Position = dto.Position;
        result.RaceStatus = dto.RaceStatus.Trim();

        await _context.SaveChangesAsync();

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteResult(int id)
    {
        var result = await _context.Results.FindAsync(id);

        if (result == null)
            return NotFound();

        _context.Results.Remove(result);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

    