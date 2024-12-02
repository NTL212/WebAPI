using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductBusinessLogic.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly IVoucherCampaignService _voucherCampaignService;
        public VoucherController(IVoucherService voucherService, IVoucherCampaignService voucherCampaignService)
        {
            _voucherService = voucherService;
            _voucherCampaignService = voucherCampaignService;

        }

        // GET api/voucher/{code}
        [HttpGet("{code}")]
        public async Task<ActionResult<Voucher>> GetVoucherByCode(string code)
        {
            var voucher = await _voucherService.GetVoucherByCode(code);
            if (voucher == null)
            {
                return NotFound("Voucher not found");
            }
            return Ok(voucher);
        }


        // POST api/voucher
        [HttpPost]
        public async Task<ActionResult<Voucher>> CreateVoucher([FromBody] VoucherDTO voucherDTO)
        {
            var createdVoucher = await _voucherService.AddAsync(voucherDTO);
            return Ok();
        }

        // PUT api/voucher
        [HttpPut]
        public async Task<ActionResult> UpdateVoucher([FromBody] VoucherDTO voucherDTO)
        {           
            var success = await _voucherService.UpdateAsync(voucherDTO);
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
            var success = await _voucherService.DeleteAsync(voucherId);
            if (success)
            {
                return NoContent();
            }
            return NotFound("Voucher not found");
        }


        [HttpGet("campaign/{voucherCampaignId}")]

        public async Task<ActionResult<VoucherCampaignDTO>> GetVoucherCampaign(int voucherCampaignId)
        {
            var voucherCp = await _voucherCampaignService.GetByIdAsync(voucherCampaignId);
            if (voucherCp != null)
            {
                return Ok(voucherCp);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("campaign")]
        public async Task<ActionResult<VoucherCampaign>> CreateVoucherCampaign([FromBody] VoucherCampaignDTO voucherDTO)
        {
            var createdVoucher = await _voucherCampaignService.AddAsync(voucherDTO);
            if (createdVoucher)
            {
                return Ok();
            }
            else
                return BadRequest("Add voucher campaign failed");
           
        }

        // PUT api/voucher
        [HttpPut("campaign")]
        public async Task<ActionResult> UpdateVoucherCampaign([FromBody] VoucherCampaignDTO voucherDTO)
        {
            var success = await _voucherCampaignService.UpdateAsync(voucherDTO);
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
            var success = await _voucherCampaignService.DeleteAsync(voucherId);
            if (success)
            {
                return NoContent();
            }
            return NotFound("Voucher not found");
        }
    }
}
