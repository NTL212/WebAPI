using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Repositories
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task<Voucher> GetVoucherByCodeAsync(string code);

        Task<bool> IsVoucherValidAsync(string code);
        Task UpdateVoucherUsageAsync(int voucherId);

        Task<Voucher> CreateVoucherAsync(Voucher voucher);

        Task<bool> DistributeVoucher(Voucher voucher, int quantity,string userIds);

        Task<PagedResult<Voucher>> GetVouchersOfUserPaged(int userId, int pageNumber, int pageSize);
    }
}
