using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Implementations
{
    public class VoucherUserRepository: Repository<VoucherUser>, IVoucherUserRepository
    {
        public VoucherUserRepository(ProductCategoryContext context) : base(context)
        {

        }

        

        public async Task<VoucherUser> GetVoucherUser(int userId, int voucherId)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.UserId == userId && v.VoucherUserId == voucherId);
        }
    }
}
