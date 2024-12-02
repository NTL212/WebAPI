using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Interfaces
{
    public interface IVoucherUserRepository:IRepository<VoucherUser>
    {
        Task<VoucherUser> GetVoucherUser(int userId, int voucherId);
    }
}
