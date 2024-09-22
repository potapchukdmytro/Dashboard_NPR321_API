using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Dashboard.BLL.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(IEnumerable<string> to, string subject, string text, bool isHtml = false)
        {
            MimeMessage message = new MimeMessage();
            message.To.AddRange(to.Select(t => InternetAddress.Parse(t)));
            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();

            if(isHtml)
            {
                bodyBuilder.HtmlBody = text;
            }
            else
            {
                bodyBuilder.TextBody = text;
            }

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendEmailAsync(string to, string subject, string text, bool isHtml = false)
        {
            await SendEmailAsync(new[] { to }, subject, text, isHtml);
        }

        public async Task SendEmailAsync(MimeMessage message)
        {
            try
            {
                string password = _configuration["MailSettings:Password"];
                string email = _configuration["MailSettings:Email"];
                string smtp = _configuration["MailSettings:SMTP"];
                int port = int.Parse(_configuration["MailSettings:Port"]);

                message.From.Add(InternetAddress.Parse(email));

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtp, port, true);
                    await client.AuthenticateAsync(email, password);
                    await client.SendAsync(message);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
