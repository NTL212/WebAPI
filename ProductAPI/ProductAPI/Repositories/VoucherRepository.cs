using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using ProductAPI.Repositories;

namespace ProductAPI.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly ProductCategoryContext _context;

        public VoucherRepository(ProductCategoryContext context)
        {
            _context = context;
        }

        public async Task<Voucher> GetVoucherByCodeAsync(string code)
        {
            return await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == code && v.Status == "active" && v.ExpiryDate > DateTime.Now);
        }

        public async Task<bool> ApplyVoucherToOrderAsync(int orderId, int voucherId, decimal discountValue)
        {
            var orderVoucher = new OrderVoucher
            {
                OrderId = orderId,
                VoucherId = voucherId,
                DiscountApplied = discountValue
            };
           
            _context.OrderVouchers.Add(orderVoucher);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsVoucherValidAsync(string code)
        {
            var voucher = await GetVoucherByCodeAsync(code);
            return voucher != null && voucher.UsedCount < voucher.MaxUsage;
        }

        public async Task UpdateVoucherUsageAsync(int voucherId)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);
            if (voucher != null)
            {
                voucher.UsedCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            voucher.ExpiryDate = DateTime.Now.AddMonths(1);
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<bool> UpdateVoucher(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<bool> DeleteVoucherAsync(int voucherId)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);
            if (voucher == null)
            {
                return false;
            }
            voucher.Status = "Inactive";
            _context.Vouchers.Update(voucher);      
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<VoucherCampaign> CreateVoucherCampaignAsync(VoucherCampaign voucher)
        {
            _context.VoucherCampaigns.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<bool> UpdateCampaignVoucher(VoucherCampaign voucher)
        {
            _context.VoucherCampaigns.Update(voucher);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId)
        {

            var voucher = await _context.VoucherCampaigns.FindAsync(voucherCampaignId);
            if (voucher == null)
            {
                return false;
            }
            voucher.Status = "Inactive";
            _context.VoucherCampaigns.Update(voucher);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<VoucherCampaign> GetVoucherCampaign(int voucherCampaignId)
        {
            VoucherCampaign vc = await _context.VoucherCampaigns.FirstOrDefaultAsync(vc => vc.CampaignId == voucherCampaignId);
            return vc;
        }
    }
}