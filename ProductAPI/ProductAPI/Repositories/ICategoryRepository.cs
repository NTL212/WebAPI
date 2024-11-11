using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> GetAll();
		Task<IEnumerable<Category>> GetAllSubCategory(int id);
		Task<Category> GetById(int id);
		Task<bool> Add(Category category);
		Task<bool> Update(Category category);
		Task<bool> Delete(int id);
	}
}
