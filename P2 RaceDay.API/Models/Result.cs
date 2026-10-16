namespace P2_RaceDay.API.Models
{
    public class RaceResult
    {
        public int ResultID { get; set; }

        public int EntryID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string RaceStatus { get; set; } = string.Empty;

        // Navigation
        public RaceEntry? Entry { get; set; }
    }
}
