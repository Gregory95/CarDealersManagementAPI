using CarsDealersManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CarsDealersManagement.Application.Services
{
    public class EmailSender(IConfiguration configuration) : IEmailSender
    {
        private readonly IConfiguration _configuration = configuration;

        public Task<bool> SendEmailAsync(List<string> email, string subject, string body)
        {
            return Execute(subject, body, email);
        }

        private Task<bool> Execute(string subject, string body, List<string> email)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("gko@donotreply.com");
                    mail.To.Add(email[0]);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    // mail.Attachments.Add(new Attachment());

                    using SmtpClient smtp = new(_configuration["EmailConfiguration:host"], 587);
                    smtp.Credentials = new NetworkCredential(_configuration["EmailConfiguration:username"], _configuration["EmailConfiguration:password"]);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                return Task.FromResult<bool>(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult<bool>(false);
            }
        }
    }
}
