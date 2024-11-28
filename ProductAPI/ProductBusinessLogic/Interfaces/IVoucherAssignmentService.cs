using ProductDataAccess.DTOs;

namespace ProductBusinessLogic.Interfaces
{
    public interface IVoucherAssignmentService:IBaseService<VoucherAssignmentDTO>
    {
        Task<VoucherAssignmentDTO> GetVoucherAssign(int voucherId, int campaignId);
    }
}
