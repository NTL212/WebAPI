namespace ProductDataAccess.Models
{
	public class EmailModel
	{
		public string FromEmail { get; set; } = string.Empty;
		public string ToEmails { get; set; } = string.Empty;  // Các email người nhận, phân cách bằng dấu ";"
		public string Subject { get; set; } = string.Empty;    // Tiêu đề email
		public string Body { get; set; } = string.Empty;       // Nội dung email
	}
}
