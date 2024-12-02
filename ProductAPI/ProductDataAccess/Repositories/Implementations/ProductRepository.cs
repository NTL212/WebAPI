using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		public ProductRepository(ProductCategoryContext context) : base(context)
		{
		}
		public async Task<IEnumerable<Product>> GetAllIncludeProducts()
		{
			return await _dbSet.Include(p=>p.Category).Where(P=>P.IsDeleted==false).ToListAsync();
		}
		
        public async Task<IEnumerable<Product>> GetAllAvailableProducts()
        {
            return await _dbSet.Include(p => p.Category).Where(P => P.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdsPagedAsync(int pageNumber, int pageSize, IEnumerable<int> ids)
        {
            return await _dbSet.Include(p => p.Category)
                .Where(P => P.IsDeleted == false && ids.Contains((int)P.CategoryId))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }

    }
}
