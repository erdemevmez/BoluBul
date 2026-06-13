using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels;
using BoluBul.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateReviewAsync(ReviewCreateViewModel model, string userId)
        {
            var businessExists = await _context.Businesses
                .AnyAsync(b => b.Id == model.BusinessId && b.IsApproved && b.IsActive);

            if (!businessExists)
            {
                throw new InvalidOperationException("İşletme bulunamadı.");
            }

            var hasReview = await _context.Reviews
                .AnyAsync(r => r.BusinessId == model.BusinessId && r.UserId == userId);

            if (hasReview)
            {
                throw new InvalidOperationException("Bu işletmeye daha önce yorum yaptınız.");
            }

            _context.Reviews.Add(new Review
            {
                BusinessId = model.BusinessId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ReviewItemViewModel>> GetUserReviewsAsync(string userId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewItemViewModel
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserDisplayName = r.Business.Name
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AdminReviewRowViewModel>> GetAdminReviewsAsync(bool pendingOnly = false)
        {
            var query = _context.Reviews
                .AsNoTracking()
                .Include(r => r.Business)
                .Include(r => r.User)
                .AsQueryable();

            if (pendingOnly)
            {
                query = query.Where(r => !r.IsApproved);
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminReviewRowViewModel
                {
                    Id = r.Id,
                    BusinessName = r.Business.Name,
                    UserName = r.User.FullName ?? r.User.Email ?? "Kullanıcı",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> ApproveReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review is null)
            {
                return false;
            }

            review.IsApproved = true;
            await _context.SaveChangesAsync();
            await RecalculateBusinessRatingAsync(review.BusinessId);

            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review is null)
            {
                return false;
            }

            var businessId = review.BusinessId;
            var wasApproved = review.IsApproved;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            if (wasApproved)
            {
                await RecalculateBusinessRatingAsync(businessId);
            }

            return true;
        }

        private async Task RecalculateBusinessRatingAsync(int businessId)
        {
            var business = await _context.Businesses.FindAsync(businessId);

            if (business is null)
            {
                return;
            }

            var approvedReviews = await _context.Reviews
                .Where(r => r.BusinessId == businessId && r.IsApproved)
                .Select(r => r.Rating)
                .ToListAsync();

            business.ReviewCount = approvedReviews.Count;
            business.AverageRating = approvedReviews.Count == 0
                ? 0
                : Math.Round((decimal)approvedReviews.Average(), 2);
            business.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
