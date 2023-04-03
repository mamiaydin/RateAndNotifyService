using System.Text;
using Newtonsoft.Json;
using NotificationService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace NotificationService;

public class NotificationListener : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public NotificationListener(string queueName)
    {
        // Establish a connection to the RabbitMQ server using the specified queue name
        // This information should be kept in appSettings.json instead of being hardcoded here
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        try
        {
            // Create a connection and channel for RabbitMQ
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;

            // Declare the queue with the specified queue name on the RabbitMQ server
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating the NotificationListener: {ex}");
            // handle the error here, maybe logging it or sending an email etc
        }

    }

    // Start listening for notifications on the RabbitMQ server
    public void StartListening(Action<Notification> onNotificationReceived)
    {
        try
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, args) =>
            {
                // Convert the received message to a Notification object
                var body = args.Body.ToArray();
                var notificationJson = Encoding.UTF8.GetString(body);
                var notification = JsonConvert.DeserializeObject<Notification>(notificationJson);

                // Call the specified delegate function to handle the received notification
                if (notification != null) onNotificationReceived(notification);
            };

            // Start consuming messages from the specified queue on the RabbitMQ server
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing a notification: {ex}");
            // handle the error here, maybe logging it or sending an email etc
        }
    }

    // Clean up resources used by the NotificationListener
    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

}