using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using uhrenWelt.Settings;
using uhrenWelt.Interfaces;

namespace uhrenWelt.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSettings> _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }


        public async Task SendEmail(string from, string to, string subject, string body)
        {
            var message = new MailMessage(
                from,
                to,
                subject,
                body
            );

            using (var emailClient = new SmtpClient(_smtpSettings.Value.Host, _smtpSettings.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(
                    _smtpSettings.Value.User,
                    _smtpSettings.Value.Password
                );
                await emailClient.SendMailAsync(message);
            }
        }

        public async Task SendEmail(string from, string to, string subject, string body, byte[] attachmentBytes)
        {
            var message = new MailMessage(
                from,
                to,
                subject,
                body
            );
            MemoryStream stream = new MemoryStream(attachmentBytes);

            var attachment = new Attachment(stream, "Invoice.pdf", "application/pdf");
            message.Attachments.Add(attachment);

            using (var emailClient = new SmtpClient(_smtpSettings.Value.Host, _smtpSettings.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(
                    _smtpSettings.Value.User,
                    _smtpSettings.Value.Password
                );
                await emailClient.SendMailAsync(message);
            }
        }
    }
}