using ProductAPI.Models;

namespace ProductAPI.Repositories
{
	public interface IProductRepository
	{
		Task<IEnumerable<Product>> GetAllProducts();
		Task<IEnumerable<Product>> GetAllProductsByCategory(int id);
		Task<Product> GetProductById(int id);
		Task<bool> AddProduct(Product product);
		Task<bool> UpdateProduct(Product product);
		Task<bool> DeleteProduct(int id);
	}
}
