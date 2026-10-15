using System.ComponentModel.DataAnnotations;    
namespace Part2_Prog6212.DTOs
{
    public class RaceDtos
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0.1, 500)]
        public decimal DistanceKm { get; set; }

        [Range(1, 100000)]
        public int Capacity { get; set; }
    }
}
