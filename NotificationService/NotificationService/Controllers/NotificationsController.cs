using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers;

// This controller handles incoming HTTP GET requests to the "/api/notifications" endpoint

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    
    // Constructor injection is used to provide an instance of the NotificationService
    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    // This action handles GET requests and returns a list of new notifications
    [HttpGet]
    public async Task<ActionResult<List<Notification>>> GetNewNotifications()
    {
        // Retrieve new notifications from the NotificationService
        var newNotifications = await _notificationService.GetNewNotificationsAsync();
        
        // Create a new request in the in-memory database to store the timestamp of when the notifications were retrieved
        var notificationRequest = new NotificationRequest
            {Guid = Guid.NewGuid(), NotificationCount = 0, Timestamp = DateTime.Now};
        await _notificationService.CreateNotificationRequestAsync(notificationRequest);
        
        // Return the list of new notifications to the client
        if (!newNotifications.Any()) return Ok("Notifications empty..");
        return Ok(newNotifications);
    }
}