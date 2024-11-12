using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using ProductAPI.Repositories;

namespace ProductAPI.Repositories
{
    public class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {

        public VoucherRepository(ProductCategoryContext context) : base(context)
        {
        }

        public async Task<Voucher> GetVoucherByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(v => v.Code == code && v.Status == "active" && v.ExpiryDate > DateTime.Now);
        }

        public async Task<bool> IsVoucherValidAsync(string code)
        {
            var voucher = await GetVoucherByCodeAsync(code);
            return voucher != null && voucher.UsedCount < voucher.MaxUsage;
        }

        public async Task UpdateVoucherUsageAsync(int voucherId)
        {
            var voucher = await _dbSet.FindAsync(voucherId);
            if (voucher != null)
            {
                voucher.UsedCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            voucher.ExpiryDate = DateTime.Now.AddMonths(1);
            _dbSet.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }


        public async Task<bool> DeleteVoucherAsync(int voucherId)
        {
            var voucher = await _dbSet.FindAsync(voucherId);
            if (voucher == null)
            {
                return false;
            }
            voucher.Status = "Inactive";
            _dbSet.Update(voucher);
            return await _context.SaveChangesAsync() > 0;
        }
	}
}
