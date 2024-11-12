using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductAPI.Repositories;
using ProductDataAccess.ViewModels;
using ProductAPI.Repositories.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IOrderVoucherRepository _orderVoucherRepository;
        private readonly IVoucherCampaignRepository _voucherCampaignRepository;
        private readonly IMapper _mapper;
        public VoucherController(IVoucherRepository voucherRepository, IMapper mapper, IOrderVoucherRepository orderVoucherRepository, IVoucherCampaignRepository voucherCampaignRepository)
        {
            _voucherRepository = voucherRepository;
            _orderVoucherRepository = orderVoucherRepository;
            _voucherCampaignRepository = voucherCampaignRepository;
            _mapper = mapper;

        }

        // GET api/voucher/{code}
        [HttpGet("{code}")]
        public async Task<ActionResult<Voucher>> GetVoucherByCode(string code)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(code);
            if (voucher == null)
            {
                return NotFound("Voucher not found");
            }
            return Ok(voucher);
        }

        // POST api/voucher/apply
        [HttpPost("apply")]
        public async Task<ActionResult> ApplyVoucherToOrder([FromBody] ApplyVoucherRequest request)
        {
            var isValid = await _voucherRepository.IsVoucherValidAsync(request.Code);
            if (!isValid)
            {
                return BadRequest("Voucher is not valid");
            }

            var voucher = await _voucherRepository.GetVoucherByCodeAsync(request.Code);
            var isApplied = await _orderVoucherRepository.ApplyVoucherToOrderAsync(request.OrderId, voucher.VoucherId, (decimal)voucher.DiscountValue);

            if (isApplied)
            {
                return Ok("Voucher applied successfully");
            }
            return BadRequest("Failed to apply voucher");
        }

        // POST api/voucher
        [HttpPost]
        public async Task<ActionResult<Voucher>> CreateVoucher([FromBody] VoucherDTO voucherDTO)
        {
            var voucher = _mapper.Map<Voucher>(voucherDTO);
            var createdVoucher = await _voucherRepository.CreateVoucherAsync(voucher);
            return CreatedAtAction(nameof(GetVoucherByCode), new { code = createdVoucher.Code }, createdVoucher);
        }

        // PUT api/voucher
        [HttpPut]
        public async Task<ActionResult> UpdateVoucher([FromBody] VoucherDTO voucherDTO)
        {
            var voucher = _mapper.Map<Voucher>(voucherDTO);
            var success = await _voucherRepository.UpdateAsync(voucher);
            if (success)
            {
                return Ok("Voucher updated successfully");
            }
            return BadRequest("Failed to update voucher");
        }

        // DELETE api/voucher/{voucherId}
        [HttpDelete("{voucherId}")]
        public async Task<ActionResult> DeleteVoucher(int voucherId)
        {
            var success = await _voucherRepository.DeleteAsync(voucherId);
            if (success)
            {
                return NoContent();
            }
            return NotFound("Voucher not found");
        }


        [HttpGet("campaign/{voucherCampaignId}")]

        public async Task<ActionResult<VoucherCampaign>> GetVoucherCampaign(int voucherCampaignId)
        {
            var voucherCp = await _voucherCampaignRepository.GetByIdAsync(voucherCampaignId);
            var voucherDto = _mapper.Map<VoucherCampaignDTO>(voucherCp);
            if (voucherDto != null)
            {
                return Ok(voucherDto);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("campaign")]
        public async Task<ActionResult<VoucherCampaign>> CreateVoucherCampaign([FromBody] VoucherCampaignDTO voucherDTO)
        {
            var voucher = _mapper.Map<VoucherCampaign>(voucherDTO);
            var createdVoucher = await _voucherCampaignRepository.AddAsync(voucher);
            if (createdVoucher)
            {
                return Ok(voucher);
            }
            else
                return BadRequest("Add voucher campaign failed");
           
        }

        // PUT api/voucher
        [HttpPut("campaign")]
        public async Task<ActionResult> UpdateVoucherCampaign([FromBody] VoucherCampaignDTO voucherDTO)
        {
            var voucher = _mapper.Map<VoucherCampaign>(voucherDTO);
            var success = await _voucherCampaignRepository.UpdateAsync(voucher);
            if (success)
            {
                return Ok("Voucher updated successfully");
            }
            return BadRequest("Failed to update voucher");
        }

        // DELETE api/voucher/{voucherId}
        [HttpDelete("{voucherId}/campaign")]
        public async Task<ActionResult> DeleteVoucherCampaign(int voucherId)
        {
            var success = await _voucherCampaignRepository.DeleteVoucherCampaignAsync(voucherId);
            if (success)
            {
                return NoContent();
            }
            return NotFound("Voucher not found");
        }

        // PUT api/voucher/{voucherId}/usage
        [HttpPut("{voucherId}/usage")]
        public async Task<ActionResult> UpdateVoucherUsage(int voucherId)
        {
            await _voucherRepository.UpdateVoucherUsageAsync(voucherId);
            return Ok("Voucher usage updated");
        }
    }
}
