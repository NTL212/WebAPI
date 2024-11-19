using OrderService.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace OrderService.Repositories
{
	public class VoucherCampaignRepository: Repository<VoucherCampaign>, IVoucherCampaignRepository
	{
		public VoucherCampaignRepository(ProductCategoryContext context) : base(context)
		{
		}

		public async Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId)
		{

			var voucher = await _dbSet.FindAsync(voucherCampaignId);
			if (voucher == null)
			{
				return false;
			}
			voucher.Status = "Inactive";
			_dbSet.Update(voucher);
			return await _context.SaveChangesAsync() > 0;
		}

	}
}
