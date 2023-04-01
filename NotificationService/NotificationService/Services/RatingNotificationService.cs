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

    public async Task<List<Rating>> GetAllNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        return notifications;
    }

    public async Task<List<Rating>> GetNewNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetNewAsync();
        return notifications;
    }

    public async Task CreateNotificationAsync(Rating rating)
    {
        await _notificationRepository.AddAsync(rating);
        await _notificationRepository.SaveChangesAsync();
    }

    public async Task CreateNotificationRequestAsync(NotificationRequest request)
    {
        await _notificationRepository.AddAsync(request);
        await _notificationRepository.SaveChangesAsync();
    }
}