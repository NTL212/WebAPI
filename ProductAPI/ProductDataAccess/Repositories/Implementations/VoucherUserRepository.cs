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

        public async Task<bool> DeleteDistributeVoucher(int id)
        {
            var vu = await _dbSet.FirstOrDefaultAsync(v=>v.VoucherUserId == id);
            try
            {
                vu.Status = false; 
                _dbSet.Update(vu);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
