namespace P2_RaceDay.API.Models
{
    public class RaceEvent
    {
        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Venue { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
