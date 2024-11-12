using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Interfaces
{
	public interface IVoucherCampaignRepository:IRepository<VoucherCampaign>
	{
		Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId);
	}
}
