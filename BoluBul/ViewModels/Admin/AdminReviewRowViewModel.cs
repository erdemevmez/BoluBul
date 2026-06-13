namespace BoluBul.ViewModels.Admin
{
    public class AdminReviewRowViewModel
    {
        public int Id { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
