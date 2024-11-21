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
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == vu.VoucherId);
            try
            {
                vu.Status = false;
                if (voucher.UsedCount > 0)
                {
                    voucher.UsedCount -= vu.Quantity;
                }             
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
