using System.ComponentModel.DataAnnotations;    

namespace Part2_Prog6212.Models
{
    public class RaceEvent
    {
        public int id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string EventType { get; set; } = string.Empty;

        public ICollection<Category> Categories { get; set; }  // Navigation property for related Categories
              = new List<Category>();
        
    }
}
