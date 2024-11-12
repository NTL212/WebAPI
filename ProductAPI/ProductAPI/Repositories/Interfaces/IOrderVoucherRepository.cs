using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Interfaces
{
	public interface IOrderVoucherRepository: IRepository<OrderVoucher>
	{
		Task<bool> ApplyVoucherToOrderAsync(int orderId, int voucherId, decimal discountValue);
	}
}
