using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface IVoucherUserRepository : IRepository<VoucherUser>
    {
        Task<bool> DeleteDistributeVoucher(int id);
    }
}
