
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService;

public class NotificationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly NotificationListener _listener;
    private readonly IServiceProvider _provider;

    public NotificationMiddleware(RequestDelegate next,string queueName, IServiceProvider provider)
    {
        _next = next;
        _provider = provider;
        _listener = new NotificationListener(queueName);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _listener.StartListening(async notification =>
            {
                using var scope = _provider.CreateScope();
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                await notificationService.CreateNotificationAsync(notification);
                // Handling the received notification here according to queueName
                
                // Retrieve an instance of AppDbContext from the DI container
            
                Console.WriteLine(notification);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex} : Can't Listen notifications, check your rabbitmq server!");
        }
     
        

        await _next(context);
    }
}