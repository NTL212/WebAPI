
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;

namespace ProductBusinessLogic.Services
{
    public class VoucherUserService : BaseService<VoucherUser, VoucherUserDTO>, IVoucherUserService
    {
        private readonly IVoucherUserRepository _voucherUserRepository;
        private readonly IVoucherRepository _voucherRepository;
        public VoucherUserService(IMapper mapper, IVoucherUserRepository repository, IVoucherRepository voucherRepository) : base(mapper, repository)
        {
            _voucherUserRepository = repository;
            _voucherRepository = voucherRepository;
        }

        public async Task<bool> DeleteDistributeVoucher(int id)
        {
            var vu = await _voucherUserRepository.GetByIdAsync(id);
            var voucher = await _voucherRepository.GetByIdAsync(vu.VoucherId);
            try
            {
                vu.Status = false;
                if (voucher.UsedCount > 0)
                {
                    voucher.UsedCount -= vu.Quantity;
                }
                _voucherUserRepository.Update(vu);
                return await _voucherUserRepository.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<VoucherUserDTO> GetVoucherUser(int voucherId, int userId)
        {
           var voucherUser =await _voucherUserRepository.GetByIdWithIncludeAsync(v=>v.VoucherId == voucherId && v.UserId ==userId);
           return _mapper.Map<VoucherUserDTO>(voucherUser);
        }
    }
}
