namespace BoluBul.ViewModels.Admin
{
    public class AdminBusinessRowViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string DistrictName { get; set; } = string.Empty;

        public bool IsApproved { get; set; }

        public bool IsActive { get; set; }

        public bool IsFeatured { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
