using Quik_BookingApp.Repos.Request;

namespace Quik_BookingApp.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest email);
    }
}
