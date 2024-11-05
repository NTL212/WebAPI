using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductAPI.DTOs;
using ProductAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ProductAPI.Services
{
	public class AuthService:IAuthService
	{
		private readonly IConfiguration _configuration;
		private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
		private readonly ProductCategoryContext _context; 

		public AuthService(IConfiguration configuration, ProductCategoryContext context)
		{
			_configuration = configuration;
			_context = context;
		}

		public string Register(RegisterDTO registerDTO)
		{
			if (registerDTO.Password != registerDTO.ConfirmPassword)
			{
				return null;
			}
			var user = new User
			{
				Username = registerDTO.UserName,
				Email = registerDTO.Email,
			};
			user.PasswordHash = _passwordHasher.HashPassword(user, registerDTO.Password); // Mã hóa mật khẩu


			_context.Users.Add(user);
			_context.SaveChanges();
			return GenerateJwtToken(user);
		}

		public string Login(LoginDTO loginDto)
		{
			var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);
			var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
			if (user == null || verificationResult== PasswordVerificationResult.Failed)
				throw new UnauthorizedAccessException("Invalid credentials");

			return GenerateJwtToken(user);
		}

		private string GenerateJwtToken(User user)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				expires: DateTime.Now.AddMinutes(1),
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
				return false; // Nếu có lỗi trong quá trình giải mã token
			}
		}

		public bool ChangePassword(ChangePasswordDTO changeDTO)
		{
			var user = _context.Users.FirstOrDefault(u=>u.Email==changeDTO.Email);
			var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changeDTO.OldPassword);
			if (changeDTO.NewPassword != changeDTO.ConfirmNewPassword || verificationResult == PasswordVerificationResult.Failed)
			{
				return false;
			}
			user.PasswordHash = _passwordHasher.HashPassword(user, changeDTO.NewPassword); // Mã hóa mật khẩu
			_context.Users.Update(user);
			return _context.SaveChanges()>0;
		}
	}
}
