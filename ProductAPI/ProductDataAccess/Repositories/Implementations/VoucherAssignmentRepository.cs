using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Implementations
{
    public class VoucherAssignmentRepository:Repository<VoucherAssignment>, IVoucherAssignmentRepository
    {
        public VoucherAssignmentRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
