
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;

namespace ProductBusinessLogic.Interfaces
{
    public interface IVoucherService:IBaseService<VoucherDTO>
    {
        Task<PagedResult<VoucherDTO>> GetVoucherPagedWithSearch(int pageNumber, int pageSize, string searchKey);
        Task<PagedResult<VoucherDTO>> GetVouchersOfUserPaged(int pageNumber, int pageSize, int userId);

        Task<bool> CreateVoucher(VoucherCreateVM createVM);
        Task<bool> UpdateVoucher(VoucherEditVM editVM);

        Task<VoucherEditVM> ConvertVoucherEditVM(VoucherDTO voucher);

        Task<bool> DistributeVoucher(VoucherDTO voucherDTO, DistributeVoucherVM distributeVoucherVM);

        Task<ValidateVoucherVM> ValidateApplyVoucher(VoucherDTO voucher, UserDTO user, List<int> productIds, decimal totalOrder);
    }
}
