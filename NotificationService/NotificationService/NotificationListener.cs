using System.Text;
using MongoDB.Driver;
using Newtonsoft.Json;
using NotificationService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


//NotificationListener which inherits IDisposable, so we can dispose it 
public class NotificationListener : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public NotificationListener(string queueName)
    {
        //this informations should be keep in appSettings.json
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        try
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;

            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating the NotificationListener: {ex}");
        }

    }

    public void StartListening(Action<Rating> onNotificationReceived)
    {
        try
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, args) =>
            {

                var body = args.Body.ToArray();
                var notificationJson = Encoding.UTF8.GetString(body);
                var notification = JsonConvert.DeserializeObject<Rating>(notificationJson);

                if (notification != null) onNotificationReceived(notification);

            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
    
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing a notification: {ex}");
            // handle the error here, maybe logging it or sending an email etc
        }
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

}