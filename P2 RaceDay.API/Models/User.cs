namespace P2_RaceDay.API.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}
