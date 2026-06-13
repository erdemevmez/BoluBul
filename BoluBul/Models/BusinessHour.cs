namespace BoluBul.Models
{
    public class BusinessHour
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }

        public Business Business { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan? OpenTime { get; set; }

        public TimeSpan? CloseTime { get; set; }

        public bool IsClosed { get; set; }
    }
}
