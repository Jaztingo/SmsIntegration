using System.Linq.Expressions;

namespace SmsIntegration.DAL.IRepository
{
    public interface IRepositoryBase<T> where T : class
    {
        Task AddAsync(
           params T[] items);
        Task AddAsyncWithoutSave(
            params T[] items);

        Task SaveChangesAsync();

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAsync(
            Expression<Func<T, bool>> where);

        Task<T> GetOneAsync(
            Expression<Func<T, bool>> where);

        Task<List<T>> GetAsync(
            Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] includes);

        Task<T> GetByIdAsync(long Id);

        void Deattach(T items);

        Task UpdateAsync(params T[] items);

        Task UpdateAsync(
            T entity,
            Expression<Func<T, bool>> where);

        Task DeleteAsync(params T[] items);

        Task DeleteAsync(
            T entity,
            Expression<Func<T, bool>> where);

        Task<int> CountAsync();

        Task<int> ExecuteAsync(
            string sql,
            params object[] parameters);

        Task<IEnumerable<TResult>> ExecuteAsync<TResult>(
            string sql,
            params object[] parameters);

        Task<bool> AnyAsync(Expression<Func<T, bool>> where);

    }
}
