using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RatingService.Models;

namespace RatingService;

public class NotificationProducer : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public NotificationProducer(string queueName)
    {
        //this informations should be keep in appSettings.json
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };
        
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _queueName = queueName;

        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishNotification(RatingNotification ratingNotification)
    {
        var notificationJson = JsonConvert.SerializeObject(ratingNotification);
        var body = Encoding.UTF8.GetBytes(notificationJson);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: properties, body: body);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}