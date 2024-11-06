﻿using ProductAPI.Models;

namespace ProductAPI.Services
{
	public interface IEmailService
	{
		Task SendEmailAsync(EmailModel emailData);
		Task<bool> ConfirmEmailAsync(string token, string email);

		Task SendConfirmationEmailAsync(string email, string token);
	}
}