using System.ComponentModel.DataAnnotations;   

namespace Part2_Prog6212.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        public int RaceEventId { get; set; }  // Foreign key to RaceEvent

        public RaceEvent? RaceEvent { get; set; }  // Navigation property to RaceEvent

    }
}
