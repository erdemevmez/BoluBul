namespace BoluBul.ViewModels
{
    public class CategoryCardViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public string Description { get; set; } = string.Empty;

        public int BusinessCount { get; set; }
    }
}
