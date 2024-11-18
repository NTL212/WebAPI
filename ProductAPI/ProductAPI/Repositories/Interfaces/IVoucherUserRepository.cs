using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Interfaces
{
    public interface IVoucherUserRepository:IRepository<VoucherUser>
    {
         Task<bool> DeleteDistributeVoucher(int id);
    }
}
