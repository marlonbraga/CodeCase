using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCase.Domain.Interfaces;
using RabbitMQ.Client;

namespace CodeCase.Domain.Services;

public class HandleHardSkillService : IHandleHardSkillService
{
    IConnection _connection;
    IModel _channel;

    public HandleHardSkillService(string hostname)
    {
        var factory = new ConnectionFactory { Uri = new Uri(hostname) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: "hello",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void SendHardSkillToQueue(string message = "Hello World!")
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: "hello",
            basicProperties: null,
            body: body);

        Console.WriteLine($" [x] Sent {message}");
    }
}
