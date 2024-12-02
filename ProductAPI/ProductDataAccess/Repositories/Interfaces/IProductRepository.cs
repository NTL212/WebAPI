using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
	public interface IProductRepository:IRepository<Product>
	{
		Task<IEnumerable<Product>> GetAllIncludeProducts();
        Task<IEnumerable<Product>> GetAllAvailableProducts();
        Task<IEnumerable<Product>> GetProductsByCategoryIdsPagedAsync(int pageNumber, int pageSize, IEnumerable<int> ids);
	}
}
