using BoluBul.Models;

namespace BoluBul.Repositories.Interfaces
{
    public interface IBusinessRepository : IGenericRepository<Business>
    {
        IQueryable<Business> QueryPublic();

        Task<Business?> GetPublicBySlugAsync(string slug);

        Task<Business?> GetWithDetailsByIdAsync(int id);
    }
}
