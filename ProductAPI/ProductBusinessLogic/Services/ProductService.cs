using AutoMapper;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Repositories;

namespace ProductBusinessLogic.Services
{
    public class ProductService : BaseService<Product, ProductDTO>, IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IMapper mapper, IProductRepository productRepository) : base(mapper, productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductDTO>> GetProductPagedByCategory(int pageNumber, int pageSize, int categoryId)
        {
            var totalRecords = await _productRepository.CountAsync(p=>p.CategoryId==categoryId);
            var products = await _productRepository.GetPagedWithIncludeSearchAsync(pageNumber, pageSize, p=>p.CategoryId==categoryId);
            return new PagedResult<ProductDTO>
            {
                Items = _mapper.Map<List<ProductDTO>>(products),
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ProductDTO>> GetProductPagedWithSearch(int pageNumber, int pageSize, string searchKey)
        {
            var searchText = searchKey.ToLower();
            var totalRecords = await _productRepository.CountAsync(p => p.ProductName.ToLower().Contains(searchText));
            var products = await _productRepository.GetPagedWithIncludeSearchAsync(pageNumber, pageSize, p => p.ProductName.ToLower().Contains(searchText));
            return new PagedResult<ProductDTO>
            {
                Items = _mapper.Map<List<ProductDTO>>(products),
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public override async Task<ProductDTO> GetByIdAsync(int id)
        {
            var product  = await _productRepository.GetByIdWithIncludeAsync(p=>p.ProductId==id, p=>p.Category);
            return _mapper.Map<ProductDTO>(product);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var product  = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.IsDeleted =true;
                _productRepository.Update(product);
                return await _productRepository.SaveChangesAsync();
            }
            return false;
        }

        public async Task<bool> RestoreProduct(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.IsDeleted = false;
                _productRepository.Update(product);
                return await _productRepository.SaveChangesAsync();
            }
            return false;
        }

        public override async Task<List<ProductDTO>> GetAllAsync()
        {
            var products = await _productRepository.GetAllIncludeProducts();
            return _mapper.Map<List<ProductDTO>>(products); 
        }

        public async Task<List<ProductDTO>> GetAllProductsByCategory(int categoryId)
        {
            var products = await _productRepository.GetAllProductsByCategory(categoryId);
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<PagedResult<ProductDTO>> GetAllProductsByCategory(int pageNumber, int pageSize, int id, string searchKey)
        {
           var products  = await _productRepository.GetPagedWithIncludeSearchAsync(pageNumber, 9, p => p.ProductName.ToLower().Contains(searchKey.ToLower()) && p.IsDeleted == false && p.Category.IsDeleted == false && (p.CategoryId == id || p.Category.ParentId == id), p => p.Category);
           var totalRecords = await _productRepository.CountAsync(p => p.ProductName.ToLower().Contains(searchKey.ToLower()) && p.IsDeleted == false && p.Category.IsDeleted == false && (p.CategoryId == id || p.Category.ParentId == id));
            return new PagedResult<ProductDTO>
            {
                TotalRecords = totalRecords,
                Items = _mapper.Map<List<ProductDTO>>(products),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ProductDTO>> GetAllProductsPaged(int pageNumber, int pageSize, string searchKey)
        {
            var products = await _productRepository.GetPagedWithIncludeSearchAsync(pageNumber, pageSize, p => p.ProductName.ToLower().Contains(searchKey.ToLower()) && p.IsDeleted == false && p.Category.IsDeleted == false, p => p.Category);
            var totalRecords = await _productRepository.CountAsync(p => p.ProductName.ToLower().Contains(searchKey.ToLower()) && p.IsDeleted == false && p.Category.IsDeleted == false);
            return new PagedResult<ProductDTO>
            {
                TotalRecords = totalRecords,
                Items = _mapper.Map<List<ProductDTO>>(products),
                PageNumber = pageNumber,
                PageSize = pageSize 
            };
        }
    }
}
