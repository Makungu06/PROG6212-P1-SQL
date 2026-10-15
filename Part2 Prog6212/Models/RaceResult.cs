using System.ComponentModel.DataAnnotations;

namespace Part2_Prog6212.Models
{
    public class RaceResult
    {
        [Required]
        public int RaceResultId { get; set; }

        public TimeSpan? FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int? Position { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Finished";

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public RaceEntry? RaceEntry { get; set; }


    }
}
