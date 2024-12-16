using System.Net.Mail;
using System.Net;

namespace AutoNet.Models.Accounts
{
    public class ConfirmationEmail
    {
        private readonly IConfiguration _configuration;

        public ConfirmationEmail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string recipientEmail, string confirmationLink)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_configuration["EmailSettings:SenderEmail"]);
                mailMessage.To.Add(new MailAddress(recipientEmail));
                mailMessage.Subject = "Confirm Your Email";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = $"Please confirm your email by clicking on the link: <a href=\"{confirmationLink}\">Confirm Email</a>";

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Credentials = new NetworkCredential(
                        _configuration["EmailSettings:SenderEmail"],
                        _configuration["EmailSettings:SenderPassword"]);
                    smtpClient.Host = _configuration["EmailSettings:SmtpHost"];
                    smtpClient.Port = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                    smtpClient.EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }
    }
}
