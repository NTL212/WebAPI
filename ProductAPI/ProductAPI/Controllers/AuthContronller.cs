
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTOs;
using ProductAPI.Filters;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthContronller : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthContronller(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public IActionResult Register(RegisterDTO registerDto)
		{
			var token = _authService.Register(registerDto);
			return Ok(new { Token = token });
		}

		[HttpPost("login")]
		public IActionResult Login(LoginDTO loginDto)
		{
			var token = _authService.Login(loginDto);
			return Ok(new { Token = token });
		}

		[HttpPost("changepass")]
		[Authorize]
		[ServiceFilter(typeof(ValidateTokenAttribute))]
		public IActionResult ChangePassword(ChangePasswordDTO changeDto)
		{
			var token = _authService.ChangePassword(changeDto);
			if (token)
			{
				return Ok();
			}
			return BadRequest();
		}
	}
}
