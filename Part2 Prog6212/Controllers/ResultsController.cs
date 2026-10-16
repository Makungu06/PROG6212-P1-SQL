using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Part2_Prog6212.Data;
using Part2_Prog6212.DTOs;
using Part2_Prog6212.Models;

namespace Part2_Prog6212.Controllers;

[ApiController]
[Route("api/results")]

public class ResultsController : ControllerBase

{
    private readonly ApplicationDbContext _db;

    public ResultsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.RaceResults
            .Include(r => r.RaceEntry)
            .ToListAsync());

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _db.RaceResults
            .Include(r => r.RaceEntry)
            .FirstOrDefaultAsync(r => r.Id == id);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(ResultRequest request)
    {
        var entryExists = await _db.RaceEntries
            .AnyAsync(e => e.Id == request.RaceEntryId);

        if (!entryExists)
            return BadRequest("Race entry does not exist.");

        var alreadyHasResult = await _db.RaceResults
            .AnyAsync(r => r.RaceEntryId == request.RaceEntryId);

        if (alreadyHasResult)
            return Conflict("A result already exists for this entry.");

        var result = new RaceResult
        {
            RaceEntryId = request.RaceEntryId,
            FinishTime = request.FinishTime,
            Position = request.Position,
            Status = request.Status
        };

        _db.RaceResults.Add(result);
        await _db.SaveChangesAsync();

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(
        int id, ResultRequest request)
    {
        var result = await _db.RaceResults.FindAsync(id);
        if (result is null) return NotFound();

        var entryExists = await _db.RaceEntries
            .AnyAsync(e => e.Id == request.RaceEntryId);

        if (!entryExists)
            return BadRequest("Race entry does not exist.");

        result.RaceEntryId = request.RaceEntryId;
        result.FinishTime = request.FinishTime;
        result.Position = request.Position;
        result.Status = request.Status;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _db.RaceResults.FindAsync(id);
        if (result is null) return NotFound();

        _db.RaceResults.Remove(result);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

}
