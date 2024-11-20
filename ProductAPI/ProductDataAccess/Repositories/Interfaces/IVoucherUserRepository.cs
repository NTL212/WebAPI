using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Interfaces
{
    public interface IVoucherUserRepository:IRepository<VoucherUser>
    {
         Task<bool> DeleteDistributeVoucher(int id);
    }
}
