using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using Newtonsoft.Json;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;

namespace ProductDataAccess.Repositories
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
    }
}
