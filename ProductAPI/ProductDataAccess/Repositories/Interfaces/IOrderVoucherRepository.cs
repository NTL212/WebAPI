using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Interfaces
{
	public interface IOrderVoucherRepository: IRepository<OrderVoucher>
	{
		Task<bool> ApplyVoucherToOrderAsync(int orderId, int voucherId, decimal discountValue);
	}
}
