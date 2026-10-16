using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Part2_Prog6212.Data;
using Part2_Prog6212.DTOs;
using Part2_Prog6212.Models;


namespace Part2_Prog6212.Controllers;

[ApiController]
[Route("api/events/{eventId:int}/races")]
public class RaceController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public RacesController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(int eventId)
    {
        return Ok(await _db.Races
            .Where(r => r.RaceEventId == eventId)
            .ToListAsync());
    }

}
 [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(
        int eventId, RaceRequest request)
    {
        var exists = await _db.RaceEvents.AnyAsync(e => e.Id == eventId);
        if (!exists) return NotFound("Event not found.");

        var race = new Race
        {
            RaceEventId = eventId,
            Name = request.Name,
            DistanceKm = request.DistanceKm,
            Capacity = request.Capacity
        };

        _db.Races.Add(race);
        await _db.SaveChangesAsync();

        return Ok(race);
    }

}
