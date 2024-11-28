using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductBusinessLogic.Interfaces;


namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[ServiceFilter(typeof(ValidateTokenAttribute))]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}/subcategories")]
        public async Task<ActionResult<List<CategoryDTO>>> GetAllSubCategory(int id)
        {
            var categories = await _categoryService.GetAllSubCategory(id);
            return Ok(categories);
        }

        [HttpGet("parentcategories")]
        public async Task<ActionResult<List<CategoryDTO>>> GetAllParentCategory()
        {
            var categories = await _categoryService.GetAllParentCategory();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> Create(CategoryDTO categoryDto)
        {
            if (await _categoryService.AddAsync(categoryDto))
            {
                return Ok(categoryDto);
            };
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CategoryDTO categoryDto)
        {
            if (id != categoryDto.CategoryId)
                return BadRequest();

            if (await _categoryService.UpdateAsync(categoryDto))
            {
                return NoContent();
            };
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _categoryService.DeleteAsync(id))
            {
                return NoContent();
            }
            return NotFound();

        }
    }
}
