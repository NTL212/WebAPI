using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Repositories
{
    public class VoucherAssignmentRepository : Repository<VoucherAssignment>, IVoucherAssignmentRepository
    {
        public VoucherAssignmentRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
