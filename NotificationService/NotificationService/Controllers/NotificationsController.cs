using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers;

public class NotificationsController : ControllerBase
{
    private readonly NotificationListener _listener;
    private readonly INotificationService _notificationService;
    
    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
        _listener = new NotificationListener("notifications_queue");
    }
    
    [HttpGet("notifications")]
    public async Task<ActionResult<List<RatingNotification>>> GetNewNotifications(DateTime? lastAccessTime)
    {

        
        if (lastAccessTime == null)
        {
            // If no last access time is provided, return all notifications
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }
        else
        {
            // Otherwise, return only the notifications submitted after the last access time
            var newNotifications = await _notificationService.GetNewNotificationsAsync(lastAccessTime.Value);
            return Ok(newNotifications);
        }
    }
    
    
}