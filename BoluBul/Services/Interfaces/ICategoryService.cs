using BoluBul.ViewModels;
using BoluBul.ViewModels.Admin;

namespace BoluBul.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryCardViewModel>> GetActiveCategoriesAsync();

        Task<IReadOnlyList<CategoryCardViewModel>> GetCategoryWithBusinessCountAsync();

        Task<CategoryListViewModel> GetCategoryListAsync();

        Task<BusinessListViewModel?> GetCategoryDetailAsync(string slug, string? sort = null);

        Task<IReadOnlyList<AdminCategoryRowViewModel>> GetAdminCategoriesAsync();

        Task<int> CreateCategoryAsync(string name, string? icon, int displayOrder);

        Task<bool> UpdateCategoryAsync(int id, string name, string? icon, int displayOrder, bool isActive);

        Task<bool> ToggleActiveAsync(int id);
    }
}
