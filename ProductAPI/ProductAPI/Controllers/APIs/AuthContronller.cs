
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductAPI.DTOs;
using ProductAPI.Filters;
using ProductAPI.Models;
using ProductAPI.Repositories;
using ProductAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContronller : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICartRepository _cartRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IEmailService _emailService;
		public AuthContronller(IAuthService authService, ICartRepository cartRepository, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _authService = authService;
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }


        //Đăng ký tài khoản =>trả về JWT Token
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDto)
        {
            var token = _authService.Register(registerDto);
            return Ok(new { Token = token });
        }

        //Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var token = _authService.Login(loginDto);
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            await _cartRepository.CreateCartIfNotExists((int)userId);
			return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Lấy userId từ session
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                // Xóa session hiện tại của người dùng
                _httpContextAccessor.HttpContext.Session.Clear();

                // Có thể thực hiện thêm các tác vụ khác nếu cần, ví dụ ghi log hoặc cập nhật trạng thái đăng xuất

                return Ok(new { Message = "Logged out successfully" });
            }
            else
            {
                return Unauthorized(new { Message = "User is not logged in" });
            }
        }

        //Đổi mật khẩu
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
