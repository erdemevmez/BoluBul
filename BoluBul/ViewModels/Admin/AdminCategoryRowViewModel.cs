namespace BoluBul.ViewModels.Admin
{
    public class AdminCategoryRowViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public int BusinessCount { get; set; }
    }
}
