
using uhrenWelt.Models;

namespace uhrenWelt.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string from, string to, string subject, string body);
        Task SendEmail(string from, string to, string subject, string body, byte[] attachmentBytes);
    }
}