using BoluBul.ViewModels;
using BoluBul.ViewModels.Admin;

namespace BoluBul.Services.Interfaces
{
    public interface IReviewService
    {
        Task CreateReviewAsync(ReviewCreateViewModel model, string userId);

        Task<IReadOnlyList<ReviewItemViewModel>> GetUserReviewsAsync(string userId);

        Task<IReadOnlyList<AdminReviewRowViewModel>> GetAdminReviewsAsync(bool pendingOnly = false);

        Task<bool> ApproveReviewAsync(int id);

        Task<bool> DeleteReviewAsync(int id);
    }
}
