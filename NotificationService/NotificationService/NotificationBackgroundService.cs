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
                    
                },listeningCancellationTokenSource.Token);
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing notifications: {ex}");
                // do nothing
            }
            //runs background service 10 seconds intervals
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
        
    }  
    
}