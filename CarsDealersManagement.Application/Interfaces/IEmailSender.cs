namespace CarsDealersManagement.Application.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(List<string> email, string subject, string body);
    }
}
