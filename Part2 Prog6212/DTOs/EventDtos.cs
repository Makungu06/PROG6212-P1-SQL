using System.ComponentModel.DataAnnotations;

namespace Part2_Prog6212.DTOs
{
    public class EventDtos
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string EventType { get; set; } = string.Empty;
    }

    public class CategoryRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}

