using ProductDataAccess.Models;

namespace OrderService.Repositories.Interfaces
{
	public interface IOrderVoucherRepository: IRepository<OrderVoucher>
	{
		Task<bool> ApplyVoucherToOrderAsync(int orderId, int voucherId, decimal discountValue);
	}
}
