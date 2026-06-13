using System.Linq.Expressions;
using BoluBul.Data;
using BoluBul.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Repositories.Implementations
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Query()
        {
            return DbSet.AsQueryable();
        }

        public Task<TEntity?> GetByIdAsync(int id)
        {
            return DbSet.FindAsync(id).AsTask();
        }

        public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = DbSet.AsQueryable();

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return query.ToListAsync();
        }

        public Task AddAsync(TEntity entity)
        {
            return DbSet.AddAsync(entity).AsTask();
        }

        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
