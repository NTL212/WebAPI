using DataAccessLayer.Models.Response;
using System.Linq.Expressions;

namespace DataAccessLayer.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithPredicateIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entity);
        void Update(T entity);
        void Delete(T entity);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();

        Task<bool> SaveChangesAsync();

        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);

        Task<IEnumerable<T>> GetPagedWithIncludeAsync(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetPagedWithIncludeSearchAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    }
}
