﻿using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
	public interface IProductRepository:IRepository<Product>
	{
		Task<IEnumerable<Product>> GetAllIncludeProducts();
        Task<IEnumerable<Product>> GetAllAvailableProducts();
        Task<IEnumerable<Product>> GetAllProductsByCategory(int id);
		Task<bool> DeleteProduct(int id);
		Task<bool> RestoreProduct(int id);
	}
}
