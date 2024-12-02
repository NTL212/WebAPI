using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.ViewModels;

namespace ProductBusinessLogic.Services
{
    public class VoucherCampaignService : BaseService<VoucherCampaign, VoucherCampaignDTO>, IVoucherCampaignService
    {
        private readonly IVoucherCampaignRepository _voucherCampaignRepository;
        private readonly IVoucherRepository _voucherRepository;

        public VoucherCampaignService(IMapper mapper, IVoucherCampaignRepository repository, IVoucherRepository voucherRepository) : base(mapper, repository)
        {
            _voucherCampaignRepository = repository;
            _voucherRepository = voucherRepository;
        }

        public async Task<bool> CreateCampaign(CampaignVM cVM)
        {
            var campaign = _mapper.Map<VoucherCampaign>(cVM);
            await _voucherCampaignRepository.AddAsync(campaign);
            return await _voucherCampaignRepository.SaveChangesAsync();
        }

        public async Task<CampaignVM> GetCampaign(int id)
        {
           var campaign = await _voucherCampaignRepository.GetByIdAsync(id);
            return _mapper.Map<CampaignVM>(campaign);
        }

        public async Task<VoucherCampaignDTO> GetCampaignWithVoucher(int id)
        {
            var campaign = await _voucherCampaignRepository.GetByIdAsync(id);
            var vouchers = await _voucherRepository.GetAllWithPredicateIncludeAsync(v => v.VoucherAssignments.Any(va => va.CampaignId == id));
            var campaignDTO = _mapper.Map<VoucherCampaignDTO>(campaign);
            campaignDTO.AssignedVouchers = _mapper.Map<List<VoucherDTO>>(vouchers);
            return campaignDTO;
        }

        public async Task<bool> UpdateCampaign(CampaignVM cVM)
        {
            var campaign = _mapper.Map<VoucherCampaign>(cVM);
            _voucherCampaignRepository.Update(campaign);
            return await _voucherCampaignRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteVoucherCampaignAsync(int voucherCampaignId)
        {

            var voucher = await _voucherCampaignRepository.GetByIdAsync(voucherCampaignId);
            if (voucher == null)
            {
                return false;
            }
            voucher.Status = "Inactive";
            _voucherCampaignRepository.Update(voucher);
            return await _voucherCampaignRepository.SaveChangesAsync();
        }
    }
}
