using System.Net;
using System.Net.Mail;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using Microsoft.Extensions.Options;

namespace DigiBuy.Application.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly SmtpSettings smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        this.smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using (var client = new SmtpClient(smtpSettings.SmtpHost, smtpSettings.SmtpPort))
        {
            client.Credentials = new NetworkCredential(smtpSettings.SmtpUser, smtpSettings.SmtpPass);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}