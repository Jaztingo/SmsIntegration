using Microsoft.EntityFrameworkCore;
using SmsIntegration.DAL.IRepository;
using System.Linq.Expressions;

namespace SmsIntegration.DAL.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly DbContext context;
        public RepositoryBase(DbContext context)
        {
            this.context = context;
        }

        #region Async Methods

        public virtual async Task AddAsync(params T[] items)
        {
            foreach (var item in items)
            {
                context.Set<T>().Add(item);
            }
            await context.SaveChangesAsync();

        }

        public virtual async Task AddAsyncWithoutSave(params T[] items)
        {
            foreach (var item in items)
            {
                context.Set<T>().Add(item);
            }
        }

        public virtual async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }


        public virtual async Task<List<T>> GetAllAsync()
        {
            List<T> query = await context.Set<T>().AsNoTracking().ToListAsync();
            return query;
        }

        public virtual async Task<List<T>> GetAsync(Expression<Func<T, bool>> where)
        {
            var query = await context.Set<T>().AsNoTracking().Where(where).ToListAsync();
            return query;
        }
        public virtual async Task<List<T>> GetAsync(
            Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] includes)
        {
            var query = context.Set<T>().AsQueryable().AsNoTracking();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(where).ToListAsync();
        }
        public virtual async Task<T> GetOneAsync(Expression<Func<T, bool>> where)
        {
            T query = await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(where);
            return query;
        }
        public virtual async Task<T> GetByIdAsync(long Id)
        {
            return await context.Set<T>().FindAsync(Id);
        }

        public virtual async Task UpdateAsync(params T[] items)
        {
            foreach (var item in items)
            {
                context.Entry(item).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }
        public virtual void Deattach(T item)
        {
            context.Entry(item).State = EntityState.Detached;
        }

        public virtual async Task UpdateAsync(T entity, Expression<Func<T, bool>> where)
        {
            var dbEntities = await GetAsync(where);

            foreach (var dbEntity in dbEntities)
            {
                var oldEntity = dbEntity;
                oldEntity = entity;

                context.Set<T>().Attach(dbEntity);

                context.Entry(oldEntity).State = EntityState.Modified;

                await context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(params T[] items)
        {
            foreach (var item in items)
            {
                context.Set<T>().Attach(item);

                context.Entry(item).State = EntityState.Deleted;
            }

            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity, Expression<Func<T, bool>> where)
        {
            var dbEntities = await GetAsync(where);

            foreach (var dbEntity in dbEntities)
            {
                context.Set<T>().Attach(dbEntity);

                context.Entry(dbEntity).State = EntityState.Deleted;

                await context.SaveChangesAsync();
            }
        }

        public virtual async Task<int> CountAsync()
        {
            return await context.Set<T>().AsNoTracking().CountAsync();
        }

        public virtual async Task<int> ExecuteAsync(string sql, params object[] parameters)
        {
            return await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public Task<IEnumerable<TResult>> ExecuteAsync<TResult>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
        {
            return await context.Set<T>().AsNoTracking().AnyAsync(where);
        }


        #endregion
    }
}
