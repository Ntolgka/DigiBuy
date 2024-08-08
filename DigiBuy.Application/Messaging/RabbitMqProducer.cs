using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace DigiBuy.Application.Messaging;

public class RabbitMqProducer
{
    private readonly string hostName = "localhost";
    private readonly string queueName = "process-email-jobs";

    public void SendEmailMessage(EmailMessage emailMessage)
    {
        var factory = new ConnectionFactory() { HostName = hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(emailMessage));

        channel.BasicPublish(exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body);
    }
}

public class EmailMessage
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}