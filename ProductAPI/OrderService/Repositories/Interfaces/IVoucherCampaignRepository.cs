using ProductDataAccess.Models;

namespace OrderService.Repositories.Interfaces
{
	public interface IVoucherCampaignRepository:IRepository<VoucherCampaign>
	{
		Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId);
	}
}
