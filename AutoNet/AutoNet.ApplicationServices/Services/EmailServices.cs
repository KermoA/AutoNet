using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

using static System.Net.Mime.MediaTypeNames;
using Multipart = MimeKit.Multipart;

namespace AutoNet.ApplicationServices.Services
{
	public class EmailServices : IEmailServices
	{
		private readonly IConfiguration _config;
		public EmailServices(IConfiguration config)
		{
			_config = config;
		}

		public async Task SendEmail(EmailDto dto)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings:SenderEmail").Value));

			foreach (var recipient in dto.To)
			{
				email.To.Add(MailboxAddress.Parse(recipient));
			}

			email.Subject = dto.Subject;

			var multipart = new Multipart("mixed");
			multipart.Add(new TextPart("html") { Text = dto.Body });

			if (dto.Attachments != null && dto.Attachments.Count > 0)
			{
				foreach (var file in dto.Attachments)
				{
					var mimePart = new MimePart
					{
						Content = new MimeContent(file.OpenReadStream()),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64,
						FileName = file.FileName
					};
					multipart.Add(mimePart);
				}
			}

			email.Body = multipart;

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(_config.GetSection("EmailSettings:SmtpHost").Value, 587, MailKit.Security.SecureSocketOptions.StartTls);
			await smtp.AuthenticateAsync(_config.GetSection("EmailSettings:SenderEmail").Value, _config.GetSection("EmailSettings:SenderPassword").Value);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);
		}

	}
}
