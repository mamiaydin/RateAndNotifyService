using NotificationService.Models;
using NotificationService.Repository;

namespace NotificationService.Services;

public class RatingNotificationService : IRatingNotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public RatingNotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Rating>> GetAllNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        return notifications;
    }

    public async Task<IEnumerable<Rating>> GetNewNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        var newNotifications = notifications.Where(x => x.IsNew);
        foreach (var rating in newNotifications)
        {
            rating.IsNew = false;
        }
        
        return newNotifications;
    }

    public async Task CreateNotificationAsync(Rating rating)
    {
        await _notificationRepository.AddAsync(rating);
        await _notificationRepository.SaveChangesAsync();
    }
}