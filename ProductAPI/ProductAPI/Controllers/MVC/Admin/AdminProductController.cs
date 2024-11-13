using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Controllers.MVC.Admin
{
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
        public async Task<IActionResult> Index(int page = 1)
        {
            var products = await _productRepository.GetPagedAsync(page, 10);
            var productsDto = _mapper.Map<PagedResult<ProductDTO>>(products);
            return View(productsDto);
        }

        public async Task<IActionResult> Detail(int id, string mess = null)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

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
                var createdProductDto = _mapper.Map<ProductDTO>(product);
                return RedirectToAction("Create", new { mess = "Success" });
            }
            else
            {
                return RedirectToAction("Create", new { mess = "Failed" });
            }
        }
    }
}
