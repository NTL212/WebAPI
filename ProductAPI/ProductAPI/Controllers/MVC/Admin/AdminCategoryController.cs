using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Models;
using ProductAPI.Filters;
using Microsoft.Extensions.Caching.Distributed;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IDistributedCache _cache;

        public AdminCategoryController(ICategoryService categoryService, IProductService productService, IDistributedCache cache)
        {
            _categoryService = categoryService;
            _productService = productService;
            _cache = cache;
        }
        public async Task<IActionResult> Index(int page = 1, string searchText = "")
        {
            var categories = await _categoryService.GetCategoryPagedWithSearch(page, 10, searchText);
            return View(categories);
        }

        public async Task<IActionResult> ProductsOfCategory(int categoryId,int page = 1)
        {
            var products = await _productService.GetProductPagedByCategory(page, 10, categoryId);
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string mess = null)
        {
            ViewData["ParentCategories"] = await _categoryService.GetAllParentCategory();
            ViewBag.Message = mess;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                await LoadParentCategoriesAsync();
                return View(categoryDTO);
            }

            try
            {
                var result = await _categoryService.AddAsync(categoryDTO);

                if (result)
                {
                    TempData["SuccessMessage"] = "Create Category Successfully";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Error occurred while creating the category.";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Create", new { id = categoryDTO.CategoryId });
            }
        }

       

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            ViewBag.ParentCategories = await _categoryService.GetAllActiveParentCategory();;
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ParentCategories"] = await _categoryService.GetAllActiveParentCategory(); ;
                return View(categoryDTO);
            }
            try
            {
                var result = await _categoryService.UpdateAsync(categoryDTO);
                if (result)
                {
                    const string cacheKey = "categories";
                    await _cache.RemoveAsync(cacheKey);
                    TempData["SuccessMessage"] = "Edit Category Successfully";
                    return RedirectToAction("Edit", new { id = categoryDTO.CategoryId });
                }
                else
                {
                    return RedirectToAction("Edit", new { id = categoryDTO.CategoryId });
                }
            }
            catch (Exception ex)
            {

                return RedirectToAction("Edit", new { id = categoryDTO.CategoryId, mess = ex.Message });
            }
        }

        // Tách logic nạp danh mục cha ra một phương thức riêng
        private async Task LoadParentCategoriesAsync()
        {
            ViewData["ParentCategories"] = await _categoryService.GetAllParentCategory();
        }
    }
}   