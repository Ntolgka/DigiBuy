using System.Net;
using System.Net.Mail;
using System.Text.Json;
using DigiBuy.Application.Messaging;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using Hangfire;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DigiBuy.Application.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly RabbitMqProducer rabbitMqProducer;
    private readonly SmtpSettings smtpSettings;

    public EmailService(RabbitMqProducer rabbitMqProducer, IOptions<SmtpSettings> smtpSettings)
    {
        this.rabbitMqProducer = rabbitMqProducer;
        this.smtpSettings = smtpSettings.Value;
    }

    public void EnqueueEmail(string toEmail, string subject, string body)
    {
        var emailMessage = new EmailMessage
        {
            ToEmail = toEmail,
            Subject = subject,
            Body = body
        };
        rabbitMqProducer.SendEmailMessage(emailMessage);
    }

    public void ProcessEmailJobs()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "process-email-jobs",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<EmailMessage>(body);
                
            // Process email
            BackgroundJob.Enqueue(() => SendEmailAsync(message.ToEmail, message.Subject, message.Body));
        };

        channel.BasicConsume(queue: "process-email-jobs",
            autoAck: true,
            consumer: consumer);
        Task.Delay(TimeSpan.FromMinutes(1));
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