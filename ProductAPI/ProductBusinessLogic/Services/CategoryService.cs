using AutoMapper;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Repositories;

namespace ProductBusinessLogic.Services
{
    public class CategoryService : BaseService<Category, CategoryDTO>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : base(mapper, categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var category = await GetByIdAsync(id);
                if (category == null && category.IsDeleted == true) return false;
                var categoryDeteled = _mapper.Map<Category>(category);
                categoryDeteled.IsDeleted = false;
                _categoryRepository.Update(categoryDeteled);
                return await _categoryRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<CategoryDTO>> GetAllActiveParentCategory()
        {
            var allParentCategory = await _categoryRepository.GetAllWithPredicateIncludeAsync(c=>c.ParentId==null && c.IsDeleted==false);
            var allParentCategoryDto = _mapper.Map<List<CategoryDTO>>(allParentCategory);
            return allParentCategoryDto;
        }

        public async Task<List<CategoryDTO>> GetAllParentCategory()
        {
            var allParentCategory = await _categoryRepository.GetAllParentCategory();
            var allParentCategoryDto = _mapper.Map<List<CategoryDTO>>(allParentCategory);
            return allParentCategoryDto;
        }

        public async Task<List<CategoryDTO>> GetAllSubCategory(int id)
        {
            var allSubCategory = await _categoryRepository.GetAllSubCategory(id);
            var allSubCategoryDto = _mapper.Map<List<CategoryDTO>>(allSubCategory);
            return allSubCategoryDto;
        }

        public async Task<PagedResult<CategoryDTO>> GetCategoryPagedWithSearch(int pageNumber, int pageSize, string searchKey)
        {
            var searchText = searchKey.ToLower();
            var totalRecords = await _categoryRepository.CountAsync(o =>o.CategoryName.ToLower().Contains(searchKey));
            var categories = await _categoryRepository.GetPagedWithIncludeSearchAsync(pageNumber, pageSize, o=>o.CategoryName.ToLower().Contains(searchText));
            return new PagedResult<CategoryDTO>
            {
                Items = _mapper.Map<List<CategoryDTO>>(categories),
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = totalRecords
            };
        }

    }
}
