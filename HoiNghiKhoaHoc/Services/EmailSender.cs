	namespace HoiNghiKhoaHoc.Services
	{
		using HoiNghiKhoaHoc.Models;
		using Microsoft.AspNetCore.Identity.UI.Services;
		using Microsoft.Extensions.Options;
		using System.Net;
		using System.Net.Mail;
		using System.Threading.Tasks;

		public class EmailSender : IEmailSender
		{
			private readonly EmailSettings _emailSettings;

			public EmailSender(IOptions<EmailSettings> options)
			{
				_emailSettings = options.Value;
			}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			try
			{
				var client = new SmtpClient(_emailSettings.SmtpServer)
				{
					Port = _emailSettings.SmtpPort,
					EnableSsl = true,
					UseDefaultCredentials = false, // 🔥 Thêm dòng này
					Credentials = new NetworkCredential(_emailSettings.Username, "")
				};


				var mailMessage = new MailMessage
				{
					From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
					Subject = subject,
					Body = htmlMessage,
					IsBodyHtml = true,
				};
				if (string.IsNullOrWhiteSpace(email))
				{
					throw new ArgumentException("Địa chỉ email nhận không được để trống.", nameof(email));
				}

				mailMessage.To.Add(email);

				await client.SendMailAsync(mailMessage);
			}
			catch (SmtpException smtpEx)
			{
				Console.WriteLine($"SMTP error: {smtpEx.StatusCode} - {smtpEx.Message}");
				throw;
			}
		}

	}

}
