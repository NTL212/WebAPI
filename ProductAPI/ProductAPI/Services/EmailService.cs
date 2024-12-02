
using Microsoft.Extensions.Options;
using ProductAPI.Email;
using ProductDataAccess.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Services
{
	public class EmailService : IEmailService
	{
		private readonly EmailSetting _emailSettings;
		private readonly IConfiguration _configuration;
		private readonly ProductCategoryContext _context;
		private readonly IUserService _userService;

		public EmailService(IOptions<EmailSetting> emailSettings, IConfiguration configuration, ProductCategoryContext context, IUserService  userService)
		{
			_emailSettings = emailSettings.Value;
			_configuration = configuration;
			_context = context;
            _userService = userService;
		}

		public async Task SendEmailAsync(EmailModel emailData)
		{
			try
			{
				var message = new MailMessage()
				{
					From = new MailAddress(emailData.FromEmail),
					Subject = emailData.Subject,
					IsBodyHtml = true,
					Body = $"""
            <html>
                <body>
                    <h3>{emailData.Body}</h3>
                </body>
            </html>
            """
				};

				foreach (var toEmail in emailData.ToEmails.Split(";"))
				{
					message.To.Add(new MailAddress(toEmail));
				}

				using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
				{
					Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
					EnableSsl = true,
				};

				await smtp.SendMailAsync(message);
			}
			catch (SmtpException ex)
			{
				// Ghi log hoặc xử lý lỗi ở đây
				throw new Exception($"SMTP Error: {ex.Message}", ex);
			}
		}

		public async Task<bool> ConfirmEmailAsync(string token, string email)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

			try
			{
				var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				var userEmail = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
				if (userEmail != email)
				{
					return false;
				}

				var user = await _userService.GetUserByEmail(email);
				if (user == null)
				{
					return false;
				}

				user.IsActive = true;
				
				return await _userService.UpdateAsync(user);
            }
			catch (Exception)
			{
				return false;
			}
		}

		public async Task SendConfirmationEmailAsync(string email, string token)
		{
			var confirmationLink = $"{_configuration["MvcUrl"]}/Account/ConfirmEmail?email={email}";

			var emailModel = new EmailModel
			{
				FromEmail = "noreply@yourapp.com",
				ToEmails = email,
				Subject = "Xác nhận email của bạn",
				Body = $"Vui lòng nhấp vào <a href='{confirmationLink}'>liên kết này</a> để xác nhận email của bạn."
			};
			await SendEmailAsync(emailModel);
		}

        public async Task<bool> SendForgotPasswordEmail(string email)
        {
			var newPass = await _userService.ForgotPassword(email);
            var emailModel = new EmailModel
            {
                FromEmail = "noreply@yourapp.com",
                ToEmails = email,
                Subject = "Molla forgot password",
                Body = $"Your new password is <h2>{newPass}</h2>"
            };
			if(newPass != "")
			{
                await SendEmailAsync(emailModel);
				return true;
            }
			return false;
        }

    }
}