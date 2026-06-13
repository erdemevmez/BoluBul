using BoluBul.Models;

namespace BoluBul.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        IQueryable<Category> QueryActive();

        Task<Category?> GetBySlugAsync(string slug);
    }
}
