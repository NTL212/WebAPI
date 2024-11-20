using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		//private readonly ProductCategoryContext _context;
		public ProductRepository(ProductCategoryContext context) : base(context)
		{
		}

		
		//public ProductRepository(ProductCategoryContext context)
		//{
		//	_context = context;
		//}

		public async Task<IEnumerable<Product>> GetAllIncludeProducts()
		{
			return await _dbSet.Include(p=>p.Category).Where(P=>P.IsDeleted==false).ToListAsync();
		}
		public async Task<IEnumerable<Product>> GetAllProductsByCategory(int id)
		{
			List<int> subcategoryIds = await _context.Set<Category>().Where(c=>c.ParentId==id).Select(c=>c.CategoryId).ToListAsync(); 
			return await _dbSet.Include(p => p.Category).Where(p=>(p.CategoryId==id||subcategoryIds.Contains((int)p.CategoryId)) && p.IsDeleted==false).ToListAsync();
		}

		public async Task<bool> DeleteProduct(int id)
		{
			var product = await _dbSet.FindAsync(id);
			if (product != null && product.IsDeleted==false)
			{
				product.IsDeleted = true;
				_dbSet.Update(product);
				return await _context.SaveChangesAsync() > 0;
			}
			return false;
		}
        public async Task<bool> RestoreProduct(int id)
        {
            var product = await _dbSet.FindAsync(id);
            if (product != null && product.IsDeleted == true)
            {
                product.IsDeleted = false;
                _dbSet.Update(product);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public Task<IEnumerable<Product>> GetAllAvailableProducts()
        {
            throw new NotImplementedException();
        }
    }
}
