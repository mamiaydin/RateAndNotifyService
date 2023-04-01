using NotificationService.Models;

namespace NotificationService.Repository;

public interface INotificationRepository
{
    Task<List<Rating>> GetAllAsync();
    Task<List<Rating>> GetNewAsync();
    Task AddAsync(Rating rating);
    Task AddAsync(NotificationRequest request);
    Task SaveChangesAsync();
}