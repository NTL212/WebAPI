using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Models;
using ProductDataAccess.ViewModels;
using ProductAPI.Filters;

namespace ProductAPI.Controllers.MVC.Admin
{
    //[JwtAuthorize("Admin")]
    //[ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminCategoryController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public AdminCategoryController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var categories = await _categoryRepository.GetPagedAsync(page, 10);
            var categoryDtos = _mapper.Map<PagedResult<CategoryDTO>>(categories);
            return View(categoryDtos);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string mess = null)
        {
            var parentCategories = await _categoryRepository.GetAllParentCategory();
            var parentCategoryDtos = _mapper.Map<List<CategoryDTO>>(parentCategories);
            ViewBag.Message = mess;
            return View(parentCategoryDtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categoryDTO)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDTO);

                if (await _categoryRepository.AddAsync(category))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Create", new { mess = "Error" });
                }
            }
            catch (Exception ex)
            {

                return RedirectToAction("Create", new { id = categoryDTO.CategoryId, mess = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string mess = null)
        {
            var categoryEVM = new AdminCategoryEditVM();
            var parentCategories = await _categoryRepository.GetAllParentCategory();
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category.ParentId != null)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync((int)category.ParentId);
                categoryEVM.ParentCategory = _mapper.Map<CategoryDTO>(parentCategory);
            }
           
            ViewBag.Message = mess;
            categoryEVM.ParentCategories = _mapper.Map<List<CategoryDTO>>(parentCategories);
            categoryEVM.Category = _mapper.Map<CategoryDTO>(category);
            return View(categoryEVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDTO categoryDTO)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDTO);

                if (await _categoryRepository.UpdateAsync(category))
                {
                    return RedirectToAction("Edit", new { id = category.CategoryId, mess = "Success" });
                }
                else
                {
                    return RedirectToAction("Edit", new { id = category.CategoryId, mess = "Error" });
                }
            }
            catch (Exception ex)
            {

                return RedirectToAction("Edit", new { id = categoryDTO.CategoryId, mess = ex.Message });
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (await _productRepository.DeleteProduct(id))
                {
                    return RedirectToAction("Detail", new { id = id, mess = "Success" });
                }
                return RedirectToAction("Detail", new { id = id, mess = "Error" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Detail", new { id = id, mess = "Error" });
            }
        }

        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                if (await _productRepository.RestoreProduct(id))
                {
                    return RedirectToAction("Detail", new { id = id, mess = "Success" });
                }
                return RedirectToAction("Detail", new { id = id, mess = "Error" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Detail", new { id = id, mess = "Error" });
            }
        }
    }
}   