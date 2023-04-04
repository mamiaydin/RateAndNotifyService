using NotificationService.Services;

namespace NotificationService;

// This class implements the IHostedService interface and defines a background service for listening to notifications.
public class NotificationBackgroundService : BackgroundService
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
        
        return Task.CompletedTask;
    }

    // This method is called when the background service is stopped.
    // It stops listening for notifications and returns a Task object that represents the ongoing operation.
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        StopListening();
        return Task.CompletedTask;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)  
    {  
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Starting NotificationListener...");
                _listener = new NotificationListener("notifications_queue");
                
                // Create a new cancellation token for this iteration
                using var listeningCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                await _listener.StartListening(async (notification) =>
                {
                    // Use the service scope factory to create a new service scope.
                    using var scope = _serviceScopeFactory.CreateScope();
                    // Get an instance of the notification service from the service provider.
                    var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                    // Call the CreateNotificationAsync method to create a new notification.
                    await notificationService.CreateNotificationAsync(notification);
                    Console.WriteLine($"Received notification: {notification}");
                    // Do something with the received notification, such as logging it or processing it
                },listeningCancellationTokenSource.Token);
                
                //if Consuming starts, runs backgroundservice at 3 min intervals because data is incosistent 
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing notifications: {ex}");
                // handle the error here, maybe logging it or sending an email etc
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
        
    }  


    // This method stops listening for notifications.
    private void StopListening()
    {
        _cancellationTokenSource.Cancel(); // Cancel the cancellation token source.
        _listener.Dispose(); // Dispose of the notification listener object.
        Console.WriteLine("Listening stopped"); // Output a message to the console.
    }
}