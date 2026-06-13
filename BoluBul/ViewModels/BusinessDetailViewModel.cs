namespace BoluBul.ViewModels
{
    public class BusinessDetailViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string CityName { get; set; } = string.Empty;

        public string DistrictName { get; set; } = string.Empty;

        public string? NeighborhoodName { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? Phone { get; set; }

        public string? WhatsApp { get; set; }

        public string? Email { get; set; }

        public string? WebsiteUrl { get; set; }

        public string Address { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? LogoUrl { get; set; }

        public string? CoverImageUrl { get; set; }

        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public bool IsFavorite { get; set; }

        public IReadOnlyList<BusinessImageViewModel> Images { get; set; } = new List<BusinessImageViewModel>();

        public IReadOnlyList<BusinessHourViewModel> Hours { get; set; } = new List<BusinessHourViewModel>();

        public IReadOnlyList<ReviewItemViewModel> Reviews { get; set; } = new List<ReviewItemViewModel>();

        public ReviewCreateViewModel ReviewForm { get; set; } = new();
    }

    public class BusinessImageViewModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? AltText { get; set; }
    }

    public class BusinessHourViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan? OpenTime { get; set; }

        public TimeSpan? CloseTime { get; set; }

        public bool IsClosed { get; set; }
    }

    public class ReviewItemViewModel
    {
        public int Id { get; set; }

        public string UserDisplayName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
