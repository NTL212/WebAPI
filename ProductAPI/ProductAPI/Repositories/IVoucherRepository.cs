using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
    public interface IVoucherRepository
    {
        Task<Voucher> GetVoucherByCodeAsync(string code);
        Task<bool> ApplyVoucherToOrderAsync(int orderId, int voucherId, decimal discountValue);
        Task<bool> IsVoucherValidAsync(string code);
        Task UpdateVoucherUsageAsync(int voucherId);

        Task<Voucher> CreateVoucherAsync(Voucher voucher);
        Task<bool> UpdateVoucher(Voucher voucher);
        Task<bool> DeleteVoucherAsync(int voucherId);

        Task<VoucherCampaign> GetVoucherCampaign(int voucherCampaignId);

        Task<VoucherCampaign> CreateVoucherCampaignAsync(VoucherCampaign voucher);
        Task<bool> UpdateCampaignVoucher(VoucherCampaign voucher);
        Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId);
    }
}
