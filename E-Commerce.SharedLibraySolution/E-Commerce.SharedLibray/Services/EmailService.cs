using Microsoft.Extensions.Configuration;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using E_Commerce.SharedLibray.Logs;

namespace E_Commerce.SharedLibray.Services
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration config;

		public EmailService(IConfiguration config)
		{
			this.config = config;
		}
		public async Task SendEmailAsync(string recipientEmail, string subject, string message)
		{
			var senderEmail = config["EmailSettings:Sender"];
			var appPassword = config["EmailSettings:Password"];
			var smtpServer = config["EmailSettings:SmtpServer"];
			var port = int.Parse(config["EmailSettings:Port"]!);

			// Create a new email message
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress(config["EmailSettings:Username"], senderEmail));
			emailMessage.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
			emailMessage.Subject = subject;
			emailMessage.Body = new TextPart("html") { Text = message };

			// Connect to the SMTP server and send the email
			using (var client = new SmtpClient())
			{
				try
				{
					await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
					await client.AuthenticateAsync(senderEmail, appPassword);
					await client.SendAsync(emailMessage);
					await client.DisconnectAsync(true);
				}
				catch (Exception ex)
				{
					//Log Original Exception
					LogException.LogExceptions(ex);

					// Display Scary-Free message to client 
					throw new Exception("Error Occured while sending email");
				}
			}
		}
	}
}
