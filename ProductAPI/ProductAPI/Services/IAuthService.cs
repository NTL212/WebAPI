using ProductAPI.DTOs;

namespace ProductAPI.Services
{
	public interface IAuthService
	{
		Task<string> Register(RegisterDTO registerDto);
		string Login(LoginDTO loginDto);

		bool ValidateToken(string token);

		bool ChangePassword(ChangePasswordDTO changeDTO);
	}
}
