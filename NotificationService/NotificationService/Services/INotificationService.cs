using NotificationService.Models;

namespace NotificationService.Services;

public interface INotificationService
{
// This method retrieves a list of new notifications asynchronously.
// It returns a Task object that represents the ongoing operation.
    Task<List<Notification>> GetNewNotificationsAsync();

// This method creates a new notification asynchronously.
// It takes a Notification object as a parameter.
// It returns a Task object that represents the ongoing operation.
    Task CreateNotificationAsync(Notification notification);

// This method creates a new notification request asynchronously.
// It takes a NotificationRequest object as a parameter.
// It returns a Task object that represents the ongoing operation.
    Task CreateNotificationRequestAsync(NotificationRequest request);
} 