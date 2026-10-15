using System.ComponentModel.DataAnnotations;    
namespace Part2_Prog6212.DTOs
{
    public class ResultDtos
    {
        [Required]
        public int RaceEntryId { get; set; }

        public TimeSpan? FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int? Position { get; set; }

        public string Status { get; set; } = "Finished";
    }
}

