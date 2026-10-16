using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Part2_Prog6212.Data;
using Part2_Prog6212.DTOs;
using Part2_Prog6212.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Part2_Prog6212.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthhContoller : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IConfiguration _config;

    public AuthController(
        ApplicationDbContext db,
        IPasswordHasher<User> hasher,
        IConfiguration config)
    {
        _db = db;
        _hasher = hasher;
        _config = config; 
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(
       RegisterRequest request)
    {
        if (request.Role != "Organiser" &&
            request.Role != "Participant")
        {
            return BadRequest(new
            {
                message = "Role must be Organiser or Participant."
            });
        }

        if (await _db.Users.AnyAsync(u =>
            u.Email == request.Email ||
            u.Username == request.Username))
        {
            return Conflict(new
            {
                message = "Username or email already exists."
            });
        }

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            FullName = request.FullName.Trim(),
            Role = request.Role
        };

        user.PasswordHash = _hasher.HashPassword(
            user, request.Password);

        if (user.Role == "Participant")
        {
            user.Participant = new Participant
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = user.Email,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Phone = request.Phone
            };
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return StatusCode(201, new
        {
            user.UserId,
            user.Username,
            user.Email,
            user.FullName,
            user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null ||
            _hasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),

            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _config["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(
                    _config["Jwt:ExpiryMinutes"] ?? "60")),
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256));

        return Ok(new
        {
            accessToken =
                new JwtSecurityTokenHandler()
                    .WriteToken(token),
            tokenType = "Bearer",
            expiresInMinutes =
                _config["Jwt:ExpiryMinutes"] ?? "60"
        });
    }
}

}
