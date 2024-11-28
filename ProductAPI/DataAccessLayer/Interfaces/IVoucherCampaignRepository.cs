using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface IVoucherCampaignRepository : IRepository<VoucherCampaign>
    {
        Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId);
    }
}
