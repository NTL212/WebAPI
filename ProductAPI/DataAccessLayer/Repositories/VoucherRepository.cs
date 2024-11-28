using DataAccessLayer.Models;
using DataAccessLayer.Models.Response;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataAccessLayer.Repositories
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
            var listUserId = JsonConvert.DeserializeObject<List<int>>(userIds);
            if (voucher.MaxUsage < quantity * listUserId.Count)
                return false;

            if (voucher.ExpiryDate < DateTime.Now)
                return false;

            if (voucher.Status == "Inactive")
                return false;

            var condition = JsonConvert.DeserializeObject<VoucherCondition>(voucher.Conditions);
            var usersWithGroups = await _context.Users
                .Include(u => u.Group)
                .Where(u => listUserId.Contains(u.UserId))
                .Select(u => new { u.UserId, u.Group.GroupName })
                .ToListAsync();

            var existingVoucherUsers = await _context.VoucherUsers
                .Where(vu => vu.VoucherId == voucher.VoucherId && listUserId.Contains(vu.UserId))
                .ToListAsync();

            var newVoucherUsers = new List<VoucherUser>();
            var updatedVoucherUsers = new List<VoucherUser>();

            foreach (var user in usersWithGroups)
            {
                var existingVoucher = existingVoucherUsers.FirstOrDefault(vu => vu.UserId == user.UserId);

                if (existingVoucher != null)
                {
                    // User đã có voucher, chỉ cần cập nhật số lượng
                    existingVoucher.Quantity += quantity;
                    updatedVoucherUsers.Add(existingVoucher);
                }
                else
                {
                    // Kiểm tra điều kiện để phân phát voucher mới
                    if (voucher.Conditions == "All" || condition.GroupName == user.GroupName)
                    {
                        newVoucherUsers.Add(new VoucherUser
                        {
                            VoucherId = voucher.VoucherId,
                            UserId = user.UserId,
                            TimesUsed = 0,
                            Quantity = quantity,
                            DateAssigned = DateTime.Now,
                            Status = true
                        });
                    }
                }
            }

            if (!newVoucherUsers.Any())
            {
                return false;
            }


            // Thêm mới và cập nhật danh sách VoucherUsers
            await _context.VoucherUsers.AddRangeAsync(newVoucherUsers);
            _context.VoucherUsers.UpdateRange(updatedVoucherUsers);

            // Cập nhật số lượng voucher còn lại
            voucher.UsedCount += quantity * listUserId.Count;
            _context.Vouchers.Update(voucher);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<PagedResult<Voucher>> GetVouchersOfUserPaged(int userId, int pageNumber, int pageSize)
        {
            var items = await _dbSet
                .Where(v => v.VoucherUsers.Any(vu => vu.UserId == userId && vu.Status == true))
                .Include(v => v.VoucherUsers)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Voucher>
            {
                Items = items,
                TotalRecords = items.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ValidateVoucherVM> ValidateVoucher(Voucher voucher, User user, List<int> productIds, decimal totalOrder)
        {
            var voucherUser = await _context.VoucherUsers.FirstOrDefaultAsync(x => x.VoucherId == voucher.VoucherId && x.UserId == user.UserId);
            var voucherAssigns = await _context.VoucherAssignments.Where(c => c.VoucherId == voucher.VoucherId).Select(v => v.CampaignId).ToListAsync();
            var campaigns = await _context.VoucherCampaigns.Where(c => voucherAssigns.Contains(c.CampaignId)).ToListAsync();

            if (campaigns != null && campaigns.Count > 0)
            {
                foreach (var campaign in campaigns)
                {
                    if (campaign.Status == "Inactive")
                    {
                        return new ValidateVoucherVM(false, "The campaign not active.");
                    }
                    if (campaign.EndDate < DateTime.Now)
                    {
                        return new ValidateVoucherVM(false, "The campaign has expired.");
                    }

                    if (campaign.TargetAudience != "All" && campaign.TargetAudience != user.Group.GroupName)
                    {
                        return new ValidateVoucherVM(false, "The campagin of voucher is not valid for your user group.");
                    }
                }
            }

            if (voucherUser.Status == false)
            {
                return new ValidateVoucherVM(false, "The voucher not active.");
            }

            if (voucherUser.Quantity == voucherUser.TimesUsed)
            {
                return new ValidateVoucherVM(false, "The voucher out of stock");
            }


            if (voucher == null)
            {
                return new ValidateVoucherVM(false, "The voucher not exist.");
            }

            // Check expiry date
            if (voucher.ExpiryDate <= DateTime.Now)
            {
                return new ValidateVoucherVM(false, "The voucher has expired.");
            }

            // Check voucher conditions
            if (voucher.Conditions != "All")
            {
                var conditions = JsonConvert.DeserializeObject<VoucherCondition>(voucher.Conditions);

                // Check user group
                if (conditions.GroupName != "All" && conditions.GroupName != user.Group.GroupName)
                {
                    return new ValidateVoucherVM(false, "The voucher is not valid for your user group.");
                }

                // Check product list
                if (conditions.ProductId?.Any() == true && productIds.Any(p => !conditions.ProductId.Contains(p)))
                {
                    return new ValidateVoucherVM(false, "The voucher is not valid for some items in your cart.");
                }

                // Check minimum order value
                if (conditions.MinOrderValue > 0 && totalOrder < conditions.MinOrderValue)
                {
                    return new ValidateVoucherVM(false, $"Your order must be at least {conditions.MinOrderValue:C} to apply this voucher.");
                }

                // Calculate discount value
                decimal discount = voucher.DiscountType == "Amount"
                    ? (decimal)voucher.DiscountValue
                    : totalOrder * (decimal)voucher.DiscountValue;

                // Check maximum discount amount
                if (conditions.MaxDiscountAmount > 0 && discount > conditions.MaxDiscountAmount)
                {
                    return new ValidateVoucherVM(false, $"The voucher supports a maximum discount of {conditions.MaxDiscountAmount:C}.");
                }
            }

            // All conditions are valid
            return new ValidateVoucherVM(true, "The voucher is valid.");
        }

    }
}
