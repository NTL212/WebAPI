using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
	public class VoucherCampaignRepository: Repository<VoucherCampaign>, IVoucherCampaignRepository
	{
		public VoucherCampaignRepository(ProductCategoryContext context) : base(context)
		{
		}
	}
}
