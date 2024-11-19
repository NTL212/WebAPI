using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductAPI.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public AdminProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(int page = 1, string searchText = null)
        {
            var products = new PagedResult<Product>();
            if (searchText != null)
            {
                products = await _productRepository.GetPagedWithIncludeSearchAsync(page, 10, p => p.ProductName.ToLower().Contains(searchText.ToLower()));
            }
            else
            {
                products = await _productRepository.GetPagedWithIncludeAsync(page, 10);
            }
            var productsDto = _mapper.Map<PagedResult<ProductDTO>>(products);
            return View(productsDto);
        }

        public async Task<IActionResult> Detail(int id, string mess = null)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            TempData["Message"] = mess;
            var productDto = _mapper.Map<ProductDTO>(product);
            return View(productDto);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string mess = null)
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Message = mess;
            ViewData["Categories"] = _mapper.Map<List<CategoryDTO>>(categories);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO productDto, IFormFile productImage)
        {
            // Lưu sản phẩm và hình ảnh vào cơ sở dữ liệu
            if (productImage != null)
            {
                var filePath = Path.Combine("wwwroot", "assets", "images", "products", productImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productImage.CopyTo(stream);
                }
                productDto.ImgName = productImage.FileName;
            }

            var product = _mapper.Map<Product>(productDto);
            product.CreatedAt = DateTime.Now;
            if (await _productRepository.AddAsync(product))
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
        public async Task<IActionResult> Edit(int id, string mess = null)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var product = await _productRepository.GetByIdWithIncludeAsync(p => p.ProductId == id, p => p.Category);
            ViewBag.Message = mess;
            ViewData["Categories"] = _mapper.Map<List<CategoryDTO>>(categories);
            var productDto = _mapper.Map<ProductDTO>(product);
            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductDTO productDTO, IFormFile productImage)
        {
            try
            {
                var product = _mapper.Map<Product>(productDTO);

                // Lưu sản phẩm và hình ảnh vào cơ sở dữ liệu
                if (productImage != null)
                {
                    var filePath = Path.Combine("wwwroot", "assets", "images", "products", productImage.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        productImage.CopyTo(stream);
                    }
                    product.ImgName = productImage.FileName;
                }

                if (await _productRepository.UpdateAsync(product))
                {
                    TempData["SuccessMessage"] = "Edit product successfull";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to edit product";

                }
                return RedirectToAction("Edit", new { id = product.ProductId });
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
                if (await _productRepository.DeleteProduct(id))
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
                if (await _productRepository.RestoreProduct(id))
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
    }
}
