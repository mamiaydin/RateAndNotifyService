using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService;

public class NotificationBackgroundService : IHostedService
{
    private readonly NotificationListener _listener;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public NotificationBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        try
        {
            _listener = new NotificationListener("notifications_queue");
        }
        catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
        {
           
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            
            _listener.StartListening(async notification =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetService<IRatingNotificationService>();
                        await notificationService.CreateNotificationAsync(notification);
                    }
                
                    Console.WriteLine(notification);
                }
            );
            return Task.CompletedTask;
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occurred while starting the NotificationBackgroundService: {ex}");
            return Task.FromException(ex);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Dispose();
        return Task.CompletedTask;
    }
}