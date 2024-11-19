using ProductDataAccess.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IVoucherUserRepository:IRepository<VoucherUser>
    {
         Task<bool> DeleteDistributeVoucher(int id);
    }
}
