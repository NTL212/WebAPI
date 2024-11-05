
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using System.Collections.Generic;

namespace ProductAPI.Repositories
{
	// Repositories/CategoryRepository.cs
	public class CategoryRepository : ICategoryRepository
	{
		private readonly ProductCategoryContext _context;

		public CategoryRepository(ProductCategoryContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Category>> GetAll()
		{
			return await _context.Categories.ToListAsync();
		}
		public async Task<IEnumerable<Category>> GetAllSubCategory(int id)
		{
			return await _context.Categories.Where(c=>c.ParentId==id).ToListAsync();
		}

		public async Task<Category> GetById(int id)
		{
			return await _context.Categories
				.FirstOrDefaultAsync(c => c.CategoryId == id);
		}

		public async Task<bool> Add(Category category)
		{
			try
			{
				await _context.Categories.AddAsync(category);
				return await _context.SaveChangesAsync() > 0; 
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> Update(Category category)
		{
			try
			{
				_context.Categories.Update(category);
				return await _context.SaveChangesAsync() > 0; 
			}
			catch (Exception ex)
			{
				return false; 
			}

		}

		public async Task<bool> Delete(int id)
		{
			try
			{
				var category = await _context.Categories.FindAsync(id);
				if (category == null && category.IsDeleted == true) return false;
				category.IsDeleted = true;
				_context.Categories.Update(category);
				return await _context.SaveChangesAsync() > 0; 
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}

}

