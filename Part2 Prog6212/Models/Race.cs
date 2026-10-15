using System.ComponentModel.DataAnnotations;   

namespace Part2_Prog6212.Models
{
    public class Race
    {
        public int Id { get; set; }
        public int RaceEventId { get; set; }  // Foreign key to RaceEvent

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }  // Distance in kilometers

        public int Capacity { get; set; }  // Maximum number of participants

        public RaceEvent? RaceEvent { get; set; }  // Navigation property to RaceEvent

        public ICollection<RaceEntry>? RaceEntries { get; set; }  // Navigation property for related RaceEntries
              = new List<RaceEntry>();
    }
}
