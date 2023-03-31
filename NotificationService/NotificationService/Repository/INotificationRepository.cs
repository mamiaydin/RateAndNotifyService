using NotificationService.Models;

namespace NotificationService.Repository;

public interface INotificationRepository
{
    Task<List<Rating>> GetAllAsync();
    Task<List<Rating>> GetNewAsync(DateTime lastAccessTime);
    Task AddAsync(Rating rating);
    Task SaveChangesAsync();
}