using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Service;

namespace Quik_BookingApp.Container
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }
        public async Task SendEmailAsync(MailRequest mailrequest)
        {
            if (string.IsNullOrEmpty(mailrequest.ToEmail))
            {
                throw new ArgumentNullException(nameof(mailrequest.ToEmail), "Recipient email cannot be null or empty.");
            }

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
            email.Subject = mailrequest.Subject ?? "No Subject";
            var builder = new BodyBuilder();


            if (System.IO.File.Exists("Attachment/dummy.pdf"))
            {
                try
                {
                    using (FileStream file = new FileStream("Attachment/dummy.pdf", FileMode.Open, FileAccess.Read))
                    {
                        byte[] fileBytes;
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add("attachment.pdf", fileBytes, ContentType.Parse("application/pdf"));  // Set correct MIME type
                        builder.Attachments.Add("attachment2.pdf", fileBytes, ContentType.Parse("application/pdf"));
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException("Error attaching file", ex);
                }
            }

            builder.HtmlBody = mailrequest.Body ?? "No content";
            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(emailSettings.Email, emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error sending email", ex);
            }
        }
    }
}
