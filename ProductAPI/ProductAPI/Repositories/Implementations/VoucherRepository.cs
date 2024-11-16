using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using ProductAPI.Repositories;
using Newtonsoft.Json;

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

        public async Task<bool> DistributeVoucher(Voucher voucher, int quantity, string userIds)
        {
            if (voucher.MaxUsage < quantity * JsonConvert.DeserializeObject<List<int>>(userIds).Count)
            {
                return false;
            }

            List<VoucherUser> vus = new List<VoucherUser>();
            List<VoucherUser> vuUpdateS = new List<VoucherUser>();
            var listUserId = JsonConvert.DeserializeObject<List<int>>(userIds);

            foreach(var id in listUserId)
            {
                var vuE = await _context.VoucherUsers.FirstOrDefaultAsync(x => x.VoucherId == voucher.VoucherId && x.UserId == id);
                if (vuE != null)
                {
                    vuE.Quantity += quantity;
                    vuUpdateS.Add(vuE);
                }
                else
                {
                    VoucherUser vu = new VoucherUser();
                    vu.VoucherId = voucher.VoucherId;
                    vu.UserId = id;
                    vu.TimesUsed = 0;
                    vu.Quantity = quantity;
                    vu.DateAssigned = DateTime.Now;
                    vu.Status = true;
                    vus.Add(vu);
                }                
            }

            await _context.VoucherUsers.AddRangeAsync(vus);
            _context.VoucherUsers.UpdateRange(vuUpdateS);

            // Trừ số lượng voucher
            voucher.MaxUsage -= quantity * listUserId.Count;
            _context.Vouchers.Update(voucher);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
