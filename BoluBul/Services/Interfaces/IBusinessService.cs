using BoluBul.Models;
using BoluBul.ViewModels;
using BoluBul.ViewModels.Admin;
using BoluBul.ViewModels.Owner;
using Microsoft.AspNetCore.Http;

namespace BoluBul.Services.Interfaces
{
    public interface IBusinessService
    {
        Task<IReadOnlyList<BusinessCardViewModel>> GetFeaturedBusinessesAsync(int take = 6);

        Task<IReadOnlyList<BusinessCardViewModel>> GetLatestBusinessesAsync(int take = 6);

        Task<BusinessListViewModel> SearchBusinessesAsync(string? search, string? category, int? districtId, string? sort);

        Task<BusinessDetailViewModel?> GetBusinessDetailBySlugAsync(string slug, string? currentUserId = null);

        Task<BusinessCreateViewModel> BuildCreateViewModelAsync(BusinessCreateViewModel? model = null);

        Task<BusinessEditViewModel?> BuildEditViewModelAsync(int id, string? userId, bool isAdmin);

        Task<int> CreateBusinessAsync(BusinessCreateViewModel model, string? ownerId, bool isAdmin);

        Task<bool> UpdateBusinessAsync(BusinessEditViewModel model, string? userId, bool isAdmin);

        Task<bool> ApproveBusinessAsync(int id);

        Task<bool> ToggleFeaturedAsync(int id);

        Task<bool> ToggleActiveAsync(int id);

        Task<Business?> GetBusinessForContactAsync(int id);

        Task<IReadOnlyList<AdminBusinessRowViewModel>> GetAdminBusinessesAsync(string? status = null);

        Task<IReadOnlyList<OwnerBusinessRowViewModel>> GetOwnerBusinessesAsync(string userId, bool isAdmin);

        Task<OwnerDashboardViewModel> GetOwnerDashboardAsync(string userId, bool isAdmin);

        Task<BusinessImageManageViewModel?> GetImageManageViewModelAsync(int businessId, string? userId, bool isAdmin);

        Task<bool> AddGalleryImageAsync(int businessId, IFormFile? file, string? altText, string? userId, bool isAdmin);

        Task<bool> DeleteGalleryImageAsync(int imageId, string? userId, bool isAdmin);

        Task<BusinessHoursViewModel?> GetHoursViewModelAsync(int businessId, string? userId, bool isAdmin);

        Task<bool> UpdateHoursAsync(BusinessHoursViewModel model, string? userId, bool isAdmin);
    }
}
