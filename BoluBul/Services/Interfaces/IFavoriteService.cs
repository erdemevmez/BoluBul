using BoluBul.ViewModels;

namespace BoluBul.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> IsFavoriteAsync(string userId, int businessId);

        Task<bool> ToggleAsync(string userId, int businessId);

        Task<IReadOnlyList<BusinessCardViewModel>> GetUserFavoritesAsync(string userId);
    }
}
