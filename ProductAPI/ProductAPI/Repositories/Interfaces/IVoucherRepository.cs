using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task<Voucher> GetVoucherByCodeAsync(string code);

        Task<bool> IsVoucherValidAsync(string code);
        Task UpdateVoucherUsageAsync(int voucherId);

        Task<Voucher> CreateVoucherAsync(Voucher voucher);

        Task<bool> DistributeVoucher(Voucher voucher, int quantity,string userIds);
    }
}
