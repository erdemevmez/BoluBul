namespace BoluBul.ViewModels.Owner
{
    public class OwnerDashboardViewModel
    {
        public int TotalBusinesses { get; set; }

        public int ApprovedBusinesses { get; set; }

        public int PendingBusinesses { get; set; }

        public int TotalViews { get; set; }

        public int PhoneClicks { get; set; }

        public int WhatsAppClicks { get; set; }

        public int DirectionClicks { get; set; }

        public IReadOnlyList<OwnerBusinessRowViewModel> Businesses { get; set; } = new List<OwnerBusinessRowViewModel>();
    }
}
