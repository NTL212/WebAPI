using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;

namespace ProductBusinessLogic.Interfaces
{
    public interface ICategoryService:IBaseService<CategoryDTO>
    {
        Task<List<CategoryDTO>> GetAllSubCategory(int id);
        Task<List<CategoryDTO>> GetAllParentCategory();
        Task<List<CategoryDTO>> GetAllActiveParentCategory();

        Task<PagedResult<CategoryDTO>> GetCategoryPagedWithSearch(int pageNumber, int pageSize, string searchKey);
    }
}
