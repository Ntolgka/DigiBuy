namespace DigiBuy.Application.Services.Interfaces;

public interface IEmailService
{
    void EnqueueEmail(string toEmail, string subject, string body);
    void ProcessEmailJobs();
    Task SendEmailAsync(string toEmail, string subject, string body);
    
}