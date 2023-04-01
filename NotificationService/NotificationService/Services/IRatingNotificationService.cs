using NotificationService.Models;

namespace NotificationService.Services;

public interface IRatingNotificationService
{
    Task<List<Rating>> GetAllNotificationsAsync();

    Task<List<Rating>> GetNewNotificationsAsync();

    Task CreateNotificationAsync(Rating rating);
    Task CreateNotificationRequestAsync(NotificationRequest request);
}