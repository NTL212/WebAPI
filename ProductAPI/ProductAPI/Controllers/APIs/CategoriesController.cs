using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductAPI.Filters;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;


namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[ServiceFilter(typeof(ValidateTokenAttribute))]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id}/subcategories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllSubCategory(int id)
        {
            var categories = await _categoryRepository.GetAllSubCategory(id);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("parentcategories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllParentCategory()
        {
            var categories = await _categoryRepository.GetAllParentCategory();
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            var categoryDto = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> Create(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            if (await _categoryRepository.AddAsync(category))
            {
                var createdCategoryDto = _mapper.Map<CategoryDTO>(category);
                return CreatedAtAction(nameof(GetById), new { id = createdCategoryDto.CategoryId }, createdCategoryDto);
            };
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CategoryDTO categoryDto)
        {
            if (id != categoryDto.CategoryId)
                return BadRequest();

            var category = _mapper.Map<Category>(categoryDto);
            if (await _categoryRepository.UpdateAsync(category))
            {
                return NoContent();
            };
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _categoryRepository.DeleteAsync(id))
            {
                return NoContent();
            }
            return NotFound();

        }
    }
}
