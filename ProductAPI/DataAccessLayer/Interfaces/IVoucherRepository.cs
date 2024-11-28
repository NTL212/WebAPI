using DataAccessLayer.Models;
using DataAccessLayer.Models.Response;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Interfaces
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task<Voucher> GetVoucherByCodeAsync(string code);

        Task<bool> IsVoucherValidAsync(string code);
        Task UpdateVoucherUsageAsync(int voucherId);

        Task<Voucher> CreateVoucherAsync(Voucher voucher);

        Task<bool> DistributeVoucher(Voucher voucher, int quantity, string userIds);

        Task<PagedResult<Voucher>> GetVouchersOfUserPaged(int userId, int pageNumber, int pageSize);


        Task<ValidateVoucherVM> ValidateVoucher(Voucher voucher, User user, List<int> productIds, decimal totalOrder);
    }
}
