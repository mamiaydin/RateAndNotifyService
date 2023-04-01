using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService;

public class NotificationBackgroundService : IHostedService
{
    private NotificationListener _listener;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public NotificationBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartListening();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        StopListening();
        return Task.CompletedTask;
    }

    private void StartListening()
    {
        Task.Run(() =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    _listener = new NotificationListener("notifications_queue");
                    _listener.StartListening(async notification =>
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var notificationService = scope.ServiceProvider.GetService<IRatingNotificationService>();
                            await notificationService.CreateNotificationAsync(notification);
                        }
                    
                        Console.WriteLine(notification);
                    });

                    Console.WriteLine("Listening started");
                    break;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                {
                    Console.WriteLine("Connection failed, retrying in 5 seconds");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
        }, _cancellationTokenSource.Token);
    }

    private void StopListening()
    {
        _cancellationTokenSource.Cancel();
        _listener.Dispose();
        Console.WriteLine("Listening stopped");
    }
}