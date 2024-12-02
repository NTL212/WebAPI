
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[ServiceFilter(typeof(ValidateTokenAttribute))]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}/category/paged/{pageNumber}")]
        public async Task<ActionResult<PagedResult<ProductDTO>>> GetAllProductsByCategory(int id, int pageNumber,string searchKey= "")
        {
            var products = await _productService.GetAllProductsByCategory(pageNumber, 9, id, searchKey);
            return Ok(products);
        }

        [HttpGet("paged/{pageNumber}")]
        public async Task<ActionResult<PagedResult<ProductDTO>>> GetAllPaged(int pageNumber, string searchKey="")
        {
            var products = await _productService.GetAllProductsPaged(pageNumber, 9, searchKey);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductDTO>> Create(ProductDTO productDto)
        {
            if (await _productService.AddAsync(productDto))
            {
                return Ok();
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

            if (await _productService.UpdateAsync(productDto))
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
            if (await _productService.DeleteAsync(id))
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
