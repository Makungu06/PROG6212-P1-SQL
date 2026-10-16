using System.Security.Claims;   
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Part2_Prog6212.Data;
using Part2_Prog6212.DTOs;

namespace Part2_Prog6212.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]

public class UsersController : ControllerBase

{
    private readonly ApplicationDbContext _db;

    public UsersController(ApplicationDbContext db)
    {
        _db = db;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
        
  {
        var user = await _db.Users
            .Where(u => u.UserId == CurrentUserId)
            .Select(u => new 
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role
            })
            .FirstOrDefaultAsync();

        return user == null ? NotFound() : Ok(user);
        {
            [HttpPut("me")]
            public async Task<IActionResult> UpdateMyProfile(
                UpdateProfileRequest request);

            {
                var User = await _db.Users.FindAsync(
                    CurrentUserId);

                if (user == null)
                    return NotFound();

                var email = Request.Email.Trim();
                .ToLowerInvariant();

                if awaits _db.Users.AnyAsync(u =>
                    u.UserId != CurrentUserId &&
                    u.Email == email))
                {
                    return Conflict(new

                    {
                        message = "Email already exists."
                    });
                }

                user.FullName = Request.FullName.Trim();
                user.Email = email;

                var participant = await _db.Participants
                    .FirstOrDefaultAsync(p =>
                        p.UserId == CurrentUserId);
                if (participant != null)
                    participant.Email = email;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.Role

                });

            }
        }













            }

        }







    
