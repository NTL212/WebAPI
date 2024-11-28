using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    // Repositories/CategoryRepository.cs
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ProductCategoryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetAllSubCategory(int id)
        {
            return await _dbSet.Where(c => c.ParentId == id).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllParentCategory()
        {
            return await _dbSet.Where(c => c.ParentId == null).ToListAsync();
        }
    }

}

