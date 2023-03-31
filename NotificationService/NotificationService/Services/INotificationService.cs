using NotificationService.Models;

namespace NotificationService.Services;

public interface INotificationService
{
    Task<List<RatingNotification>> GetAllNotificationsAsync();

    Task<List<RatingNotification>> GetNewNotificationsAsync(DateTime lastAccessTime);

    Task CreateNotificationAsync(RatingNotification ratingNotification);
}