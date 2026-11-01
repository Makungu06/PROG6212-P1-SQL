using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using P2_RaceDay.API.Data;
using P2_RaceDay.API.DTOs;
using P2_RaceDay.API.Models;


namespace P2_RaceDay.API.Controllers;

[ApiController]
[Route("api/auth")]

public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegistrationDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var role = dto.Role.Trim();

        if (role != "Organiser" && role != "Participant")
            return BadRequest(
                "Role must be Organiser or Participant.");

        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(
            u => u.Email.ToLower() == email))
        {
            return Conflict("Email is already registered.");
        }

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = email,
            FullName = dto.FullName.Trim(),
            Role = role,
            Password = BCrypt.Net.BCrypt.HashPassword(
                dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return StatusCode(201, new
        {
            message = "Registration successful.",
            userId = user.UserID,
            user.Email,
            user.FullName,
            user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.Email.ToLower() == email);

        if (user == null ||
            !BCrypt.Net.BCrypt.Verify(
                dto.Password, user.Password))
        {
            return Unauthorized(
                "Invalid email or password.");
        }

        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(key) ||
            string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
        {
            return StatusCode(500,
                "JWT configuration is incomplete.");
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var expiryMinutes = int.TryParse(
            _configuration["Jwt:ExpiryMinutes"],
            out var minutes) ? minutes : 60;

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                expiryMinutes),
            signingCredentials: credentials);

        return Ok(new
        {
            message = "Login successful.",
            token = new JwtSecurityTokenHandler()
                .WriteToken(token),
            expiresAt = token.ValidTo,
            user = new
            {
                user.UserID,
                user.Email,
                user.FullName,
                user.Role
            }
        });

    }
}
