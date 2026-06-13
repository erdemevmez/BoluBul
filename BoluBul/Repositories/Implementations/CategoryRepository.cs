using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IQueryable<Category> QueryActive()
        {
            return Context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder);
        }

        public Task<Category?> GetBySlugAsync(string slug)
        {
            return Context.Categories
                .Include(c => c.Businesses)
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }
    }
}
