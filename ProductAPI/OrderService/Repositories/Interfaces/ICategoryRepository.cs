using ProductDataAccess.Models;

namespace OrderService.Repositories
{
	public interface ICategoryRepository:IRepository<Category>
	{
		Task<IEnumerable<Category>> GetAllSubCategory(int id);
        Task<IEnumerable<Category>> GetAllParentCategory();
        Task<bool> DeleteAsync(int id);
	}
}
