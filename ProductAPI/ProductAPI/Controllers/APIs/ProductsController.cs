
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductAPI.Filters;
using ProductDataAccess.Models;
using ProductAPI.Repositories;

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
            var products = await _productRepository.GetAllProducts();
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


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await _productRepository.GetProductById(id);
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
            if (await _productRepository.AddProduct(product))
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
            if (await _productRepository.UpdateProduct(product))
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
