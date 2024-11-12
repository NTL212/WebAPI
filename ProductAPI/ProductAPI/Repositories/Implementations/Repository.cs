
using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly ProductCategoryContext _context;
		public readonly DbSet<T> _dbSet;

        public Repository(ProductCategoryContext context)
        {
           _context = context;
           _dbSet = context.Set<T>();
        }
        public async Task<bool> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

		public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize)
		{
			var totalRecords = await _dbSet.CountAsync();
			var items = await _dbSet
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<T>
			{
				Items = items,
				TotalRecords = totalRecords,
				PageNumber = pageNumber,
				PageSize = pageSize
			};
		}
	}
}
