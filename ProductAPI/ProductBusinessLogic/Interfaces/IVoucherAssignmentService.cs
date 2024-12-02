using ProductDataAccess.DTOs;

namespace ProductBusinessLogic.Interfaces
{
    public interface IVoucherAssignmentService:IBaseService<VoucherAssignmentDTO>
    {
        Task<VoucherAssignmentDTO> GetVoucherAssign(int voucherId, int campaignId);

        Task<bool> CreateAssignments(List<int> voucherIds, int campaignId);
    }
}
