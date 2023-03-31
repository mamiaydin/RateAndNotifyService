using NotificationService.Models;
using NotificationService.Repository;

namespace NotificationService.Services;

public class NotificationConsumerService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationConsumerService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<RatingNotification>> GetAllNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        return notifications;
    }

    public async Task<List<RatingNotification>> GetNewNotificationsAsync(DateTime lastAccessTime)
    {
        var newNotifications = await _notificationRepository.GetNewAsync(lastAccessTime);
        return newNotifications;
    }

    public async Task CreateNotificationAsync(RatingNotification ratingNotification)
    {
        await _notificationRepository.AddAsync(ratingNotification);
        await _notificationRepository.SaveChangesAsync();
    }
}