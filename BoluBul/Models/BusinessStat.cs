namespace BoluBul.Models
{
    public class BusinessStat
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }

        public Business Business { get; set; } = null!;

        public int ViewCount { get; set; }

        public int PhoneClickCount { get; set; }

        public int WhatsAppClickCount { get; set; }

        public int DirectionClickCount { get; set; }

        public DateTime? LastViewedAt { get; set; }
    }
}
