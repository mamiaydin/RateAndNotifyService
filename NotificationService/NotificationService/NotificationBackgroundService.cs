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
        catch(Exception ex)
        {
            
        }
       
    }

    public Task StartAsync(CancellationToken cancellationToken)
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Dispose();
        return Task.CompletedTask;
    }
}