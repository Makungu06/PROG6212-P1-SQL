namespace P2_RaceDay.API.Models
{
    public class Participant
    {
        public int ParticipantID { get; set; }

        public int UserID { get; set; }

        public string FullName { get; set; } = string.Empty;
    }
}
