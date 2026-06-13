using System.Linq.Expressions;

namespace BoluBul.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> Query();

        Task<TEntity?> GetByIdAsync(int id);

        Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        Task SaveChangesAsync();
    }
}
