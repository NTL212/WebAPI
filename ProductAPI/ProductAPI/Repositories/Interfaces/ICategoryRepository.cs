using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
	public interface ICategoryRepository:IRepository<Category>
	{
		Task<IEnumerable<Category>> GetAllSubCategory(int id);
        Task<IEnumerable<Category>> GetAllParentCategory();
        Task<bool> DeleteAsync(int id);
	}
}
