using AutoMapper;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;

namespace ProductBusinessLogic.Services
{
    public class VoucherAssignmentService : BaseService<VoucherAssignment, VoucherAssignmentDTO>, IVoucherAssignmentService
    {
        private readonly IVoucherAssignmentRepository _voucherAssignmentRepository;
        public VoucherAssignmentService(IMapper mapper, IVoucherAssignmentRepository repository) : base(mapper, repository)
        {
            _voucherAssignmentRepository = repository;
        }

        public async Task<VoucherAssignmentDTO> GetVoucherAssign(int voucherId, int campaignId)
        {
            var voucherAssignment = await _voucherAssignmentRepository.GetByIdWithIncludeAsync(v=>v.VoucherId==voucherId && v.CampaignId==campaignId);
            return _mapper.Map<VoucherAssignmentDTO>(voucherAssignment);
        }
    }
}
