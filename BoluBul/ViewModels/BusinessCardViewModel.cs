namespace BoluBul.ViewModels
{
    public class BusinessCardViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string DistrictName { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? LogoUrl { get; set; }

        public string? Phone { get; set; }

        public string? WhatsApp { get; set; }

        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public bool IsFeatured { get; set; }

        public string Address { get; set; } = string.Empty;

        public string OpenStatusText { get; set; } = "Bilgi al";
    }
}
