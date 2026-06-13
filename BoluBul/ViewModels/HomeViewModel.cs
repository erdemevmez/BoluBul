namespace BoluBul.ViewModels
{
    public class HomeViewModel
    {
        public IReadOnlyList<CategoryCardViewModel> Categories { get; set; } = new List<CategoryCardViewModel>();

        public IReadOnlyList<BusinessCardViewModel> FeaturedBusinesses { get; set; } = new List<BusinessCardViewModel>();

        public IReadOnlyList<BusinessCardViewModel> LatestBusinesses { get; set; } = new List<BusinessCardViewModel>();

        public IReadOnlyList<DiscoveryCardViewModel> DiscoveryCards { get; set; } = new List<DiscoveryCardViewModel>();

        public int RegisteredBusinessCount { get; set; }

        public int CategoryCount { get; set; }

        public int ReviewCount { get; set; }

        public int FeaturedBusinessCount { get; set; }
    }

    public class DiscoveryCardViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string CssClass { get; set; } = string.Empty;
    }
}
