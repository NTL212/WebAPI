
using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using System.Linq.Expressions;

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
            try
            {
                await _dbSet.AddAsync(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
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
            try
            {
                _dbSet.Update(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
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

        public async Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<PagedResult<T>> GetPagedWithIncludeAsync(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var totalRecords = await _dbSet.CountAsync();
            var items = await query
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

        public async Task<PagedResult<T>> GetPagedWithIncludeSearchAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {

            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var items = await query
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalRecords = items.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<T>> GetAllWithPredicateIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<bool> AddRangeAsync(List<T> entity)
        {
            try
            {
                await _dbSet.AddRangeAsync(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
