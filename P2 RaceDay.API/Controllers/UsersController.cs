using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2_RaceDay.API.Data;


namespace P2_RaceDay.API.Controllers;
    public class UpdateProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

[ApiController]
[Route("api/users")]
[Authorize]


    public class UsersController : ControllerBase
    {
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier),
            out userId);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var user = await _context.Users
            .Where(u => u.UserID == userId)
            .Select(u => new
            {
                u.UserID,
                u.Username,
                u.Email,
                u.FullName,
                u.Role
            })
            .FirstOrDefaultAsync();

        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        UpdateProfileDto dto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.FullName) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            !new System.ComponentModel.DataAnnotations
                .EmailAddressAttribute()
                .IsValid(dto.Email))
        {
            return BadRequest(
                "A valid email and full name are required.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserID == userId);

        if (user == null)
            return NotFound();

        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(
            u => u.UserID != userId &&
                 u.Email.ToLower() == email))
        {
            return Conflict("Email is already in use.");
        }

        user.FullName = dto.FullName.Trim();
        user.Email = email;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Profile updated successfully.",
            user.UserID,
            user.Username,
            user.Email,
            user.FullName,
            user.Role
        });
    }

}

