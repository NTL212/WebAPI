
using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using System.Collections.Generic;

namespace ProductAPI.Repositories
{
	// Repositories/CategoryRepository.cs
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		public CategoryRepository(ProductCategoryContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Category>> GetAllSubCategory(int id)
		{
			return await _dbSet.Where(c=>c.ParentId==id).ToListAsync();
		}

		public async Task<bool> DeleteAsync(int id)
		{
			try
			{
				var category = await _dbSet.FindAsync(id);
				if (category == null && category.IsDeleted == true) return false;
				category.IsDeleted = true;
				_dbSet.Update(category);
				return await _context.SaveChangesAsync() > 0; 
			}
			catch (Exception ex)
			{
				return false;
			}
		}

        public async Task<IEnumerable<Category>> GetAllParentCategory()
        {
            return await _dbSet.Where(c => c.ParentId == null).ToListAsync();
        }
    }

}

