namespace BoluBul.ViewModels
{
    public class BusinessHoursViewModel
    {
        public int BusinessId { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        public List<BusinessHourEditItemViewModel> Hours { get; set; } = new();
    }

    public class BusinessHourEditItemViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan? OpenTime { get; set; }

        public TimeSpan? CloseTime { get; set; }

        public bool IsClosed { get; set; }
    }
}
