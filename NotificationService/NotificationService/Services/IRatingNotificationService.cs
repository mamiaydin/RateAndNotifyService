using NotificationService.Models;

namespace NotificationService.Services;

public interface IRatingNotificationService
{
    Task<IEnumerable<Rating>> GetAllNotificationsAsync();

    Task<IEnumerable<Rating>> GetNewNotificationsAsync();

    Task CreateNotificationAsync(Rating rating);
}