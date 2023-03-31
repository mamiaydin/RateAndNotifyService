using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller}")]
public class NotificationsController : ControllerBase
{
    private readonly IRatingNotificationService _ratingNotificationService;
    
    public NotificationsController(IRatingNotificationService ratingNotificationService)
    {
        _ratingNotificationService = ratingNotificationService;
    }
    
    [HttpGet("new_notifications")]
    public async Task<ActionResult<List<Rating>>> GetNewNotifications()
    {
        // Otherwise, return only the notifications submitted after the last access time
            var newNotifications = await _ratingNotificationService.GetNewNotificationsAsync();
            return Ok(newNotifications);
    }
    
    [HttpGet("notifications")]
    public async Task<ActionResult<List<Rating>>> GetAllNotifications()
    {
        // Otherwise, return only the notifications submitted after the last access time
        var notifications = await _ratingNotificationService.GetAllNotificationsAsync();
        return Ok(notifications);
    }
}