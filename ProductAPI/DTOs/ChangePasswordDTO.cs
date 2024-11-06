namespace ProductAPI.DTOs
{
	public class ChangePasswordDTO
	{
		public string Email { get; set; }
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmNewPassword { get; set; }
	}
}
