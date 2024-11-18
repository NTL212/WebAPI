using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Implementations
{
    public class VoucherAssignmentRepository:Repository<VoucherAssignment>, IVoucherAssignmentRepository
    {
        public VoucherAssignmentRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
