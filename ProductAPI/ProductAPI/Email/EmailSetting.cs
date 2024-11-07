namespace ProductAPI.Email
{
	public class EmailSetting
	{
		public string SmtpServer { get; set; } = string.Empty;
		public int SmtpPort { get; set; }
		public string SmtpUser { get; set; } = string.Empty;
		public string SmtpPass { get; set; } = string.Empty;
	}
}
