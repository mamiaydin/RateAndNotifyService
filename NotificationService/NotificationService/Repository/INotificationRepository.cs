using NotificationService.Models;

namespace NotificationService.Repository;

public interface INotificationRepository
{
    Task<List<Notification>> GetNewNotificationsAsync();
    Task AddAsync(Notification notification);
    Task AddAsync(NotificationRequest request);
    Task SaveChangesAsync();
}