using System.ComponentModel.DataAnnotations;    

namespace P2_RaceDay.API.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
