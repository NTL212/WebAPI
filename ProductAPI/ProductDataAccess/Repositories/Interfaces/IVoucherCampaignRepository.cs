using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Interfaces
{
	public interface IVoucherCampaignRepository:IRepository<VoucherCampaign>
	{
		Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId);
	}
}
