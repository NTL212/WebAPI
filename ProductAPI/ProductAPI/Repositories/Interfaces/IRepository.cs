using ProductDataAccess.Models.Response;
using System.Linq.Expressions;

namespace ProductAPI.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithPredicateIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(int id);
        Task<bool> AddAsync(T entity);
        Task<bool> AddRangeAsync(List<T> entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);

        Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize);

        Task<PagedResult<T>> GetPagedWithIncludeAsync(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes);
        Task<PagedResult<T>> GetPagedWithIncludeSearchAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    }
}
