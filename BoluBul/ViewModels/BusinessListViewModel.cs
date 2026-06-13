namespace BoluBul.ViewModels
{
    public class BusinessListViewModel
    {
        public string? Search { get; set; }

        public string? Category { get; set; }

        public int? DistrictId { get; set; }

        public string Sort { get; set; } = "newest";

        public int TotalCount { get; set; }

        public IReadOnlyList<BusinessCardViewModel> Businesses { get; set; } = new List<BusinessCardViewModel>();

        public IReadOnlyList<CategoryCardViewModel> Categories { get; set; } = new List<CategoryCardViewModel>();

        public IReadOnlyList<DistrictFilterViewModel> Districts { get; set; } = new List<DistrictFilterViewModel>();
    }

    public class DistrictFilterViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
