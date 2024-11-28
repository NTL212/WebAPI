using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using Microsoft.Extensions.Caching.Distributed;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminProductController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public AdminProductController(IProductService productService, ICategoryService categoryService, IDistributedCache cache)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cache = cache;
        }
        public async Task<IActionResult> Index(int page = 1, string searchText = "")
        {
            var products = await _productService.GetProductPagedWithSearch(page, 10, searchText);
            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = await _categoryService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO productDto, IFormFile productImage)
        {
            if (!ModelState.IsValid)
            {
                return await ReturnModelError(productDto);
            }

            // Lưu sản phẩm và hình ảnh vào cơ sở dữ liệu
            if (productImage != null)
            {
                if (!productImage.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("", "The uploaded file is not a valid image.");
                    return await ReturnModelError(productDto);
                }

                if (productImage.Length > 5 * 1024 * 1024) // 2MB
                {
                    ModelState.AddModelError("", "Image size must be less than 2MB.");
                    return await ReturnModelError(productDto);
                }

                var filePath = Path.Combine("wwwroot", "assets", "images", "products", productImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productImage.CopyTo(stream);
                }
                productDto.ImgName = productImage.FileName;
            }

            productDto.CreatedAt = DateTime.Now;
            if (await _productService.AddAsync(productDto))
            {
                TempData["SuccessMessage"] = "Add product successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add product";

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            ViewData["Categories"] = await _categoryService.GetAllAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductDTO productDTO, IFormFile productImage=null)
            {

            if (!ModelState.IsValid) {
                return await ReturnModelError(productDTO);
            }

            try
            {
                // Lưu sản phẩm và hình ảnh vào cơ sở dữ liệu
                if (productImage != null)
                {
                    if (!productImage.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("", "The uploaded file is not a valid image.");
                        return await ReturnModelError(productDTO);
                    }

                    if (productImage.Length > 5 * 1024 * 1024) // 2MB
                    {
                        ModelState.AddModelError("", "Image size must be less than 2MB.");
                        return await ReturnModelError(productDTO);
                    }

                    var filePath = Path.Combine("wwwroot", "assets", "images", "products", productImage.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        productImage.CopyTo(stream);
                    }
                    productDTO.ImgName = productImage.FileName;
                }

                if (await _productService.UpdateAsync(productDTO))
                {
                    TempData["SuccessMessage"] = "Edit product successfull";
                    string cacheKey = $"product:{productDTO.ProductId}";
                    await _cache.RemoveAsync(cacheKey);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to edit product";

                }
                return RedirectToAction("Edit", new { id = productDTO.ProductId });
            }
            catch (Exception ex)
            {

                return RedirectToAction("Edit", new { id = productDTO.ProductId, mess = ex.Message });
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (await _productService.DeleteAsync(id))
                {
                    TempData["SuccessMessage"] = "Delete product successfull";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete product";

                }
                return RedirectToAction("Detail", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error";
                return RedirectToAction("Detail", new { id = id });
            }
        }

        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                if (await _productService.RestoreProduct(id))
                {
                    TempData["SuccessMessage"] = "Restore product successfull";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to Restore product";

                }
                return RedirectToAction("Detail", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error";
                return RedirectToAction("Detail", new { id = id });
            }
        }

        private async Task<IActionResult> ReturnModelError(ProductDTO productDTO)
        {
            ViewData["Categories"] = await _categoryService.GetAllAsync();
            return View(productDTO);
        }
    }
}
