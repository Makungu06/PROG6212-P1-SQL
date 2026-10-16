using System.ComponentModel.DataAnnotations;


namespace Part2_Prog6212.Models
{
    public class RaceEntry
    {
        public int RaceEntryId { get; set; }
        public int ParticipantId { get; set; }

        public int RaceId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Enrolled"; 

        public Participant? Participant { get; set; }  // Navigation property to Participant

        public Race? Race { get; set; }

        public RaceResult? Result { get; set; }
    }
}
