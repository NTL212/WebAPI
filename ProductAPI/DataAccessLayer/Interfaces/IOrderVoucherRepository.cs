using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface IOrderVoucherRepository : IRepository<OrderVoucher>
    {
        Task<bool> ApplyVoucherToOrderAsync(int orderId, int voucherId, decimal discountValue);
    }
}
