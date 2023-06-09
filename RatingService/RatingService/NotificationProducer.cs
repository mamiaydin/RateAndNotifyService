﻿using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RatingService.Models;

namespace RatingService;

public class NotificationProducer : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    // Constructor for creating a new instance of NotificationProducer, which connects to RabbitMQ and sets up a queue to send notifications to
    public NotificationProducer(string queueName)
    {
        try
        {
            // RabbitMQ connection factory to connect to RabbitMQ instance running on local machine
            var rabbitHostName = Environment.GetEnvironmentVariable("RABBIT_HOSTNAME");
            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitHostName ?? "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "password"
            };

            // Create a connection to RabbitMQ using connection factory
            _connection = connectionFactory.CreateConnection();

            // Create a channel on the connection to RabbitMQ
            _channel = _connection.CreateModel();

            // Name of the queue to which notifications will be sent
            _queueName = queueName;

            // Declare the queue with specified name on the channel
            _channel.QueueDeclare(
                queue: _queueName, // Name of queue 
                durable: true, // Set queue to be durable, meaning it will survive a RabbitMQ server restart
                exclusive: false, // Set queue to be non-exclusive, meaning other channels can access it
                autoDelete: false, // Set queue to not be automatically deleted when all consumers have disconnected
                arguments: null // Optional additional arguments for queue declaration
            );
        }
        catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
        {
            Console.WriteLine("Failed to connect to RabbitMQ server: " + ex.Message);
            throw;
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
        {
            Console.WriteLine("RabbitMQ operation interrupted: " + ex.Message);
            throw;
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine("Queue name parameter cannot be null: " + ex.Message);
            throw;
        }
        catch (RabbitMQ.Client.Exceptions.ChannelAllocationException ex)
        {
            Console.WriteLine("Failed to allocate a channel: " + ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed : " + ex.Message);
            throw;
        }
    }

    // Method to publish a notification of generic type T to the RabbitMQ queue
    public void PublishNotification<T>(T notification)
    {
        if (_channel == null) return;
        // Serialize the notification object to JSON format
        var notificationJson = JsonConvert.SerializeObject(notification);

        // Convert JSON string to bytes
        var body = Encoding.UTF8.GetBytes(notificationJson);

        // Create basic properties for the message to set delivery mode as persistent
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        // Publish the message to the queue on the channel with given routing key and properties
        _channel.BasicPublish(
            exchange: "", // No exchange is being used for this implementation
            routingKey: _queueName, // Name of the queue to which message will be published
            basicProperties: properties, // Basic properties of message
            body: body // Body of message to publish
        );
    }

    // Dispose method to release resources used by NotificationProducer
    public void Dispose()
    {
        if (_channel == null || _connection == null) return;
        _channel.Dispose(); // Dispose the channel
        _connection.Dispose(); // Dispose the connection
    }
}