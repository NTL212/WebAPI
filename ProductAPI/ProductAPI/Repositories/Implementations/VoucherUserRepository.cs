using Microsoft.EntityFrameworkCore;
using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Implementations
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
