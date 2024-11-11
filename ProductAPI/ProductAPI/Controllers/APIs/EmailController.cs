using Microsoft.AspNetCore.Mvc;
using ProductAPI.Services;
using ProductDataAccess.Models.Request;

namespace ProductAPI.Controllers.APIs
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailController : ControllerBase
	{
		private readonly IEmailService _emailService;

		public EmailController(IEmailService emailService)
		{
			_emailService = emailService;
		}

		[HttpPost("ConfirmEmail")]
		public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
		{
			var isConfirmed = await _emailService.ConfirmEmailAsync(request.Token, request.Email);
			if (isConfirmed)
			{
				return Ok("Xác nhận email thành công!");
			}

			return BadRequest("Token không hợp lệ hoặc đã hết hạn.");
		}
	}
}
