using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBusinessLogic.Interfaces
{
    public interface IProductService: IBaseService<ProductDTO>
    {
        Task<PagedResult<ProductDTO>> GetProductPagedByCategory(int pageNumber, int pageSize, int categoryId);
        Task<PagedResult<ProductDTO>> GetProductPagedWithSearch(int pageNumber, int pageSize, string searchKey);

        Task<bool> RestoreProduct(int productId);
        Task<List<ProductDTO>> GetAllProductsByCategory(int categoryId);
        Task<PagedResult<ProductDTO>> GetAllProductsByCategory(int pageNumber, int pageSize, int id, string searchKey);
        Task<PagedResult<ProductDTO>> GetAllProductsPaged(int pageNumber, int pageSize, string searchKey);
    }
}
