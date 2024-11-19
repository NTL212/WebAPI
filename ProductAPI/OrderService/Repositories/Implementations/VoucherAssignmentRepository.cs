using OrderService.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace OrderService.Repositories.Implementations
{
    public class VoucherAssignmentRepository:Repository<VoucherAssignment>, IVoucherAssignmentRepository
    {
        public VoucherAssignmentRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
