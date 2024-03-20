using RabbitMQ.Client;
namespace DLS_Backend.utility;
using System.Text;

public class RabbitMQService
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    /// <summary>
    /// Constructor for the RabbitMQService class that sets up the connection to the RabbitMQ server and creates a queue 
    /// </summary>
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

        _channel.QueueDeclare(queue: "UserQueue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    /// <summary>
    /// Sends a message to the RabbitMQ server
    /// </summary>
    /// <param type="string" name="message"></param>
    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
            routingKey: "UserQueue",
            basicProperties: null,
            body: body);
    }

    public void Close()
    {
        _connection.Close();
    }
}
