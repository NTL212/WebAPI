using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllSubCategory(int id);
        Task<IEnumerable<Category>> GetAllParentCategory();
    }
}
