using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
	public interface ICategoryRepository:IRepository<Category>
	{
		Task<IEnumerable<Category>> GetAllSubCategory(int id);
        Task<IEnumerable<Category>> GetAllParentCategory();
	}
}
