using RabbitMQ.Client;
namespace DLS_Backend.utility;
using System.Text;

public class RabbitMQService
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService()
    {
        _factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
        };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "testQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
            routingKey: "testQueue",
            basicProperties: null,
            body: body);
    }

    public void Close()
    {
        _connection.Close();
    }
}
