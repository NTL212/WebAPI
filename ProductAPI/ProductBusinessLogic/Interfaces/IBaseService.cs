using ProductDataAccess.Models.Response;
using System.Drawing;

namespace ProductBusinessLogic.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        Task<bool> AddAsync(T entity);
        Task<bool> AddRangeAsync(List<T> entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> UpdateRangeAsync(List<T> entity);
        Task<bool> DeleteAsync(int id);

        Task<T> GetByIdAsync(int id);

        Task<int> GetCountAsync();

        Task<List<T>> GetAllAsync();

        Task<PagedResult<T>> GetAllPagedAsync(int pageNumber, int pageSize);
    }
}
