using System.ComponentModel.DataAnnotations;    

namespace Part2_Prog6212.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
       
        [Required]
        public string Role { get; set; } = "Participant"; // Default role is "User"

    }
}
