using NotificationService.Services;

namespace NotificationService;

// This class implements the IHostedService interface and defines a background service for listening to notifications.
public class NotificationBackgroundService : IHostedService
{
    private NotificationListener _listener; // The notification listener object.
    private readonly IServiceScopeFactory _serviceScopeFactory; // The service scope factory object.
    private readonly CancellationTokenSource _cancellationTokenSource; // The cancellation token source object.

    // The constructor for the NotificationBackgroundService class.
    // It takes an IServiceScopeFactory object as a parameter.
    public NotificationBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    // This method is called when the background service is started.
    // It starts listening for notifications and returns a Task object that represents the ongoing operation.
    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartListening();
        return Task.CompletedTask;
    }

    // This method is called when the background service is stopped.
    // It stops listening for notifications and returns a Task object that represents the ongoing operation.
    public Task StopAsync(CancellationToken cancellationToken)
    {
        StopListening();
        return Task.CompletedTask;
    }

    // This method starts listening for notifications.
    private void StartListening()
    {
        // This code runs in a background thread.
        Task.Run(() =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    // Create a new notification listener object and start listening for notifications.
                    _listener = new NotificationListener("notifications_queue");
                    _listener.StartListening(async notification =>
                    {
                        // Use the service scope factory to create a new service scope.
                        using var scope = _serviceScopeFactory.CreateScope();
                        // Get an instance of the notification service from the service provider.
                        var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                        // Call the CreateNotificationAsync method to create a new notification.
                        await notificationService.CreateNotificationAsync(notification);
                    });

                    Console.WriteLine("Listening started");
                    break;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                {
                    Console.WriteLine("Connection failed, retrying in 5 seconds"); // Output an error message to the console.
                    Thread.Sleep(TimeSpan.FromSeconds(5)); // Wait for 5 seconds before retrying.
                }
            }
        }, _cancellationTokenSource.Token);
    }

    // This method stops listening for notifications.
    private void StopListening()
    {
        _cancellationTokenSource.Cancel(); // Cancel the cancellation token source.
        _listener.Dispose(); // Dispose of the notification listener object.
        Console.WriteLine("Listening stopped"); // Output a message to the console.
    }
}