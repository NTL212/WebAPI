using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Models;
using ProductDataAccess.ViewModels;
using ProductAPI.Filters;
using MailKit.Search;
using Microsoft.Extensions.Caching.Distributed;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminCategoryController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public AdminCategoryController(IProductRepository productRepository, ICategoryRepository categoryRepository, IDistributedCache cache, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _cache = cache;
        }
        public async Task<IActionResult> Index(int page = 1, string searchText = "")
        {
            var categories = await _categoryRepository.GetPagedWithIncludeSearchAsync(page, 10, p => p.CategoryName.ToLower().Contains(searchText.ToLower()));
            var categoryDtos = _mapper.Map<PagedResult<CategoryDTO>>(categories);
            return View(categoryDtos);
        }

        public async Task<IActionResult> ProductsOfCategory(int categoryId,int page = 1)
        {
            var products = new PagedResult<Product>();
            products = await _productRepository.GetPagedWithIncludeSearchAsync(page, 10, p=>p.CategoryId==categoryId);
            var productsDto = _mapper.Map<PagedResult<ProductDTO>>(products);
            return View(productsDto);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string mess = null)
        {
            var parentCategories = await _categoryRepository.GetAllParentCategory();
            ViewData["ParentCategories"] = _mapper.Map<List<CategoryDTO>>(parentCategories);
            ViewBag.Message = mess;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                var parentCategories = await _categoryRepository.GetAllParentCategory();
                ViewData["ParentCategories"] = _mapper.Map<List<CategoryDTO>>(parentCategories);
                return View(categoryDTO);
            }
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
            var parentCategories = await _categoryRepository.GetAllParentCategory();
            var category = await _categoryRepository.GetByIdAsync(id);
         
            ViewBag.Message = mess;
            parentCategories = parentCategories.Where(c => c.IsDeleted == false);
            ViewBag.ParentCategories = _mapper.Map<List<CategoryDTO>>(parentCategories);
            var categoryDTO= _mapper.Map<CategoryDTO>(category);
            return View(categoryDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                var parentCategories = await _categoryRepository.GetAllParentCategory();
                parentCategories = parentCategories.Where(c => c.IsDeleted == false);
                ViewData["ParentCategories"] = _mapper.Map<List<CategoryDTO>>(parentCategories);
                return View(categoryDTO);
            }
            try
            {
                var category = _mapper.Map<Category>(categoryDTO);

                if (await _categoryRepository.UpdateAsync(category))
                {
                    const string cacheKey = "categories";
                    await _cache.RemoveAsync(cacheKey);
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
                    const string cacheKey = "categories";
                    await _cache.RemoveAsync(cacheKey);
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
                    const string cacheKey = "categories";
                    await _cache.RemoveAsync(cacheKey);
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