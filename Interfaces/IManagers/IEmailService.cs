namespace Interfaces.IManagers
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(string toEmail, string subject, string body);
    }
}
