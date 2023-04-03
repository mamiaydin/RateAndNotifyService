using NotificationService.Models;
using NotificationService.Repository;

namespace NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    

    public async Task<List<Notification>> GetNewNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetNewNotificationsAsync();
        return notifications;
    }

    public async Task CreateNotificationAsync(Notification notification)
    {
        try
        {
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Notification data is inconsistent so dont have to handle it
            Console.WriteLine($"An error occurred while creating the notification: {ex.Message}");
        }
    }

    public async Task CreateNotificationRequestAsync(NotificationRequest request)
    {
        await _notificationRepository.AddAsync(request);
        await _notificationRepository.SaveChangesAsync();
    }
}