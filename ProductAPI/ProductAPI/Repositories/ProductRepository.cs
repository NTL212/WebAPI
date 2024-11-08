﻿using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;

namespace ProductAPI.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ProductCategoryContext _context;

		public ProductRepository(ProductCategoryContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Product>> GetAllProducts()
		{
			return await _context.Products.Include(p=>p.Category).Where(P=>P.IsDeleted==false).ToListAsync();
		}
		public async Task<IEnumerable<Product>> GetAllProductsByCategory(int id)
		{
			List<int> subcategoryIds = await _context.Categories.Where(c=>c.ParentId==id).Select(c=>c.CategoryId).ToListAsync(); 
			return await _context.Products.Include(p => p.Category).Where(p=>p.CategoryId==id||subcategoryIds.Contains((int)p.CategoryId)).ToListAsync();
		}
		public async Task<Product> GetProductById(int id)
		{
			return await _context.Products.FindAsync(id);
		}

		public async Task<bool> AddProduct(Product product)
		{
			_context.Products.Add(product);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateProduct(Product product)
		{
			_context.Products.Update(product);
			return await _context.SaveChangesAsync()>0;
		}

		public async Task<bool> DeleteProduct(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product != null && product.IsDeleted==false)
			{
				product.IsDeleted = true;
				_context.Products.Update(product);
				return await _context.SaveChangesAsync() > 0;
			}
			return false;
		}
	}
}
