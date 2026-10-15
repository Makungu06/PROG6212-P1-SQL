using System.ComponentModel.DataAnnotations;    

namespace Part2_Prog6212.Models
{
    public class Participant
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }  

        public ICollection<RaceEntry>? RaceEntries { get; set; }  // Navigation property for related RaceEntries
              = new List<RaceEntry>();


    }
}
