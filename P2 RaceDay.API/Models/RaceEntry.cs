namespace P2_RaceDay.API.Models
{
    public class RaceEntry
    {
        public int EntryID { get; set; }

        public int RaceID { get; set; }

        public int ParticipantID { get; set; }

        public DateTime EntryDate { get; set; }

        public string Status { get; set; } = string.Empty;

        // Navigation properties
        public Race? Race { get; set; }
        public Participant? Participant { get; set; }
    }
}
