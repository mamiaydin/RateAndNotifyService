using NotificationService.Models;

namespace NotificationService.Repository;

public interface INotificationRepository
{
    Task<List<RatingNotification>> GetAllAsync();
    Task<List<RatingNotification>> GetNewAsync(DateTime lastAccessTime);
    Task AddAsync(RatingNotification ratingNotification);
    Task SaveChangesAsync();
}