using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IRatingNotificationService _ratingNotificationService;
    
    public NotificationsController(IRatingNotificationService ratingNotificationService)
    {
        _ratingNotificationService = ratingNotificationService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Rating>>> GetNewNotifications()
    {
        var newNotifications = await _ratingNotificationService.GetNewNotificationsAsync();
        
        //create new request in memory db so that I can store Timestamp
        var newRequest = new NotificationRequest
            {Guid = new Guid(), NotificationCount = newNotifications.Count, Timestamp = DateTime.Now};
        await _ratingNotificationService.CreateNotificationRequestAsync(newRequest);
        
        return Ok(newNotifications);
    }
    

}