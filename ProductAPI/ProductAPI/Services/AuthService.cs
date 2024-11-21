using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductAPI.Services
{
	public class AuthService:IAuthService
	{
		private readonly IConfiguration _configuration;
		private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
		private readonly ProductCategoryContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IEmailService _emailService;
		public AuthService(IConfiguration configuration, ProductCategoryContext context, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
		{
			_configuration = configuration;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_emailService = emailService;
		}

		public async Task<AuthResponseData> Register(RegisterDTO registerDTO)
		{
            var userE = _context.Set<User>().FirstOrDefault(u => u.Email == registerDTO.Email);

            if (userE != null||registerDTO.Password != registerDTO.ConfirmPassword)
			{
                throw new UnauthorizedAccessException("Invalid register");
            }
			var user = new User
			{
				Username = registerDTO.UserName,
				Email = registerDTO.Email,
				IsActive = false
			};
			user.PasswordHash = _passwordHasher.HashPassword(user, registerDTO.Password); // Mã hóa mật khẩu

			// Tạo token xác nhận email
			var emailConfirmationToken = GenerateEmailConfirmationToken(user);

			await _emailService.SendConfirmationEmailAsync(user.Email, emailConfirmationToken);

			var userRole = new UserRole()
			{
				RoleId = 2,
				UserId = user.UserId,
			};
			_context.Set<User>().Add(user);
			_context.Set<UserRole>().Add(userRole);
			_context.SaveChanges();
			var response = new AuthResponseData(emailConfirmationToken, user.UserId, user.Username);
            return response;
		}

		public async Task<AuthResponseData> Login(LoginDTO loginDto)
		{
			try
			{
				var user = await _context.Set<User>().Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive == true);
				var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
				if (user == null || verificationResult == PasswordVerificationResult.Failed)
					throw new UnauthorizedAccessException("Invalid credentials");
				var token = GenerateJwtToken(user);
				var authData = new AuthResponseData(token, user.UserId, user.Username, user.Role.Name);
				return authData;
			}
			catch (Exception ex) {
				return null;
			}
		}

		private string GenerateEmailConfirmationToken(User user)
		{
			// Tạo token đơn giản bằng JWT
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim("UserId", user.UserId.ToString())
				}),
				Expires = DateTime.UtcNow.AddHours(24), // Token hết hạn sau 24 giờ
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		private string GenerateJwtToken(User user)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo Claims
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Name)  // Role được thêm vào claim
        };

            var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				expires: DateTime.Now.AddMinutes(10),
				claims: claims,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public bool ValidateToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			try
			{
				var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

				if (jwtToken == null)
				{
					return false; // Token không hợp lệ
				}

				// Kiểm tra thời gian hết hạn
				var expirationTime = jwtToken.ValidTo; // Lấy thời gian hết hạn
				if (expirationTime < DateTime.UtcNow) // So sánh với thời gian hiện tại
				{
					return false; // Token đã hết hạn
				}

				return true; // Token hợp lệ và chưa hết hạn
			}
			catch (Exception)
			{
				return false; 
			}
		}

		public bool ChangePassword(ChangePasswordDTO changeDTO)
		{
			var user = _context.Set<User>().FirstOrDefault(u=>u.Email==changeDTO.Email);
			var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changeDTO.OldPassword);
			if (changeDTO.NewPassword != changeDTO.ConfirmNewPassword || verificationResult == PasswordVerificationResult.Failed)
			{
				return false;
			}
			user.PasswordHash = _passwordHasher.HashPassword(user, changeDTO.NewPassword); // Mã hóa mật khẩu
			_context.Set<User>().Update(user);
			return _context.SaveChanges()>0;
		}
	}
}
