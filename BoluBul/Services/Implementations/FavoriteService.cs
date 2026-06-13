using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> IsFavoriteAsync(string userId, int businessId)
        {
            return _context.Favorites.AnyAsync(f => f.UserId == userId && f.BusinessId == businessId);
        }

        public async Task<bool> ToggleAsync(string userId, int businessId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BusinessId == businessId);

            var added = favorite is null;

            if (favorite is null)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    BusinessId = businessId,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                _context.Favorites.Remove(favorite);
            }

            await _context.SaveChangesAsync();
            return added;
        }

        public async Task<IReadOnlyList<BusinessCardViewModel>> GetUserFavoritesAsync(string userId)
        {
            return await _context.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .Include(f => f.Business)
                    .ThenInclude(b => b.Category)
                .Include(f => f.Business)
                    .ThenInclude(b => b.District)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new BusinessCardViewModel
                {
                    Id = f.Business.Id,
                    Name = f.Business.Name,
                    Slug = f.Business.Slug,
                    CategoryName = f.Business.Category.Name,
                    DistrictName = f.Business.District.Name,
                    ShortDescription = f.Business.ShortDescription,
                    LogoUrl = f.Business.LogoUrl,
                    Phone = f.Business.Phone,
                    WhatsApp = f.Business.WhatsApp,
                    AverageRating = f.Business.AverageRating,
                    ReviewCount = f.Business.ReviewCount,
                    IsFeatured = f.Business.IsFeatured,
                    Address = f.Business.Address
                })
                .ToListAsync();
        }
    }
}
