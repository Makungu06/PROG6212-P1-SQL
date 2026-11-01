
namespace P3_RaceDay.MVC.Models.DTOs;

public class LoginResponseDto
{
    public string Message { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public LoginUserDto User { get; set; } = new();
}

public class LoginUserDto
{
    public int UserID { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
