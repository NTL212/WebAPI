
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductAPI.Filters;
using ProductAPI.Services;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContronller : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IEmailService _emailService;
		public AuthContronller(IAuthService authService, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }


        //Đăng ký tài khoản =>trả về JWT Token
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDto)
        {
            var authData = _authService.Register(registerDto);
            var response = new ApiResponse<AuthResponseData>("success", "Register Success", authData.Result);
            return Ok(response);
        }

        //Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
			var authData = _authService.Login(loginDto);
            if (authData.Result == null) 
            {
                return BadRequest(new ApiResponse<bool>("error", "Login faied.", false));
            }
            var response = new ApiResponse<AuthResponseData>("success", "Login successful.",authData.Result);
			return Ok(response);
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
