using AutoMapper;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
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

        public async Task<bool> CreateAssignments(List<int> voucherIds, int campaignId)
        {
            List<VoucherAssignment> listVA = new List<VoucherAssignment>();
            foreach (int voucherId in voucherIds)
            {
                VoucherAssignment va = new VoucherAssignment();
                va.VoucherId = voucherId;
                va.CampaignId = campaignId;
                listVA.Add(va);
            }
            await _voucherAssignmentRepository.AddRangeAsync(listVA);
            return await _voucherAssignmentRepository.SaveChangesAsync();
        }

        public async Task<VoucherAssignmentDTO> GetVoucherAssign(int voucherId, int campaignId)
        {
            var voucherAssignment = await _voucherAssignmentRepository.GetByIdWithIncludeAsync(v=>v.VoucherId==voucherId && v.CampaignId==campaignId);
            return _mapper.Map<VoucherAssignmentDTO>(voucherAssignment);
        }


    }
}
