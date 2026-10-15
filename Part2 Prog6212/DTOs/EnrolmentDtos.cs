using System.ComponentModel.DataAnnotations;

namespace Part2_Prog6212.DTOs
{
    public class EnrolmentDtos
    {
        [Range(1, int.MaxValue)]
        public int RaceId { get; set; }
    }
}
