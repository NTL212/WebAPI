using ProductDataAccess.DTOs;
namespace ProductBusinessLogic.Interfaces
{
    public interface IVoucherUserService:IBaseService<VoucherUserDTO>
    {
        Task<VoucherUserDTO> GetVoucherUser(int voucherId, int userId);
        Task<bool> DeleteDistributeVoucher(int id);

        Task<List<VoucherUserDTO>> GetAllVoucherOfUser(int userId);
    }
}
