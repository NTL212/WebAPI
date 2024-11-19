
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductAPI.Filters;
using ProductDataAccess.Models;
using ProductAPI.Repositories;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[ServiceFilter(typeof(ValidateTokenAttribute))]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
        {
            var products = await _productRepository.GetAllIncludeProducts();
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{id}/category")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductsByCategory(int id)
        {
            var products = await _productRepository.GetAllProductsByCategory(id);
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{id}/category/paged/{pageNumber}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductsByCategory(int id, int pageNumber,string searchKey= "")
        {
            var products = await _productRepository.GetPagedWithIncludeSearchAsync(pageNumber, 9, p => p.ProductName.ToLower().Contains(searchKey.ToLower()) && (p.CategoryId==id || p.Category.ParentId == id), p => p.Category);
            var productsDto = _mapper.Map<PagedResult<ProductDTO>>(products);
            return Ok(productsDto);
        }

        [HttpGet("paged/{pageNumber}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllPaged(int pageNumber, string searchKey="")
        {
            var products = await _productRepository.GetPagedWithIncludeSearchAsync(pageNumber, 9,p=>p.ProductName.ToLower().Contains(searchKey.ToLower()) ,p=>p.Category);
            var productsDto = _mapper.Map<PagedResult<ProductDTO>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductDTO>> Create(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            if (await _productRepository.AddAsync(product))
            {
                var createdProductDto = _mapper.Map<ProductDTO>(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProductDto.ProductId }, createdProductDto);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(int id, ProductDTO productDto)
        {
            if (id != productDto.ProductId)
                return BadRequest();

            var product = _mapper.Map<Product>(productDto);
            if (await _productRepository.UpdateAsync(product))
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _productRepository.DeleteProduct(id))
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
