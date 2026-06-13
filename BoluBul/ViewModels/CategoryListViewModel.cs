namespace BoluBul.ViewModels
{
    public class CategoryListViewModel
    {
        public IReadOnlyList<CategoryCardViewModel> Categories { get; set; } = new List<CategoryCardViewModel>();
    }
}
