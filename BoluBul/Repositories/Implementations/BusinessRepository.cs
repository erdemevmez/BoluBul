using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Repositories.Implementations
{
    public class BusinessRepository : GenericRepository<Business>, IBusinessRepository
    {
        public BusinessRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IQueryable<Business> QueryPublic()
        {
            return Context.Businesses
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.City)
                .Include(b => b.District)
                .Include(b => b.Stats)
                .Where(b => b.IsApproved && b.IsActive);
        }

        public Task<Business?> GetPublicBySlugAsync(string slug)
        {
            return Context.Businesses
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.City)
                .Include(b => b.District)
                .Include(b => b.Neighborhood)
                .Include(b => b.Images.OrderBy(i => i.DisplayOrder))
                .Include(b => b.Hours)
                .Include(b => b.Reviews.Where(r => r.IsApproved))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Slug == slug && b.IsApproved && b.IsActive);
        }

        public Task<Business?> GetWithDetailsByIdAsync(int id)
        {
            return Context.Businesses
                .Include(b => b.Category)
                .Include(b => b.City)
                .Include(b => b.District)
                .Include(b => b.Neighborhood)
                .Include(b => b.Images.OrderBy(i => i.DisplayOrder))
                .Include(b => b.Hours)
                .Include(b => b.Stats)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
