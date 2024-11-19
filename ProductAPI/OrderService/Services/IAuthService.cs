using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;

namespace OrderService.Services
{
	public interface IAuthService
	{
		Task<AuthResponseData> Register(RegisterDTO registerDto);
		Task<AuthResponseData> Login(LoginDTO loginDto);

		bool ValidateToken(string token);

		bool ChangePassword(ChangePasswordDTO changeDTO);
	}
}
