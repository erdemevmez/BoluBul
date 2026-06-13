namespace BoluBul.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalBusinesses { get; set; }

        public int PendingBusinesses { get; set; }

        public int TotalUsers { get; set; }

        public int TotalReviews { get; set; }

        public int TotalCategories { get; set; }

        public IReadOnlyList<AdminBusinessRowViewModel> LatestBusinesses { get; set; } = new List<AdminBusinessRowViewModel>();

        public IReadOnlyList<AdminReviewRowViewModel> LatestReviews { get; set; } = new List<AdminReviewRowViewModel>();
    }
}
