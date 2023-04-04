using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }
    

    public async Task<List<Notification>> GetNewNotificationsAsync()
    {
        //get lastTimestamp value from request table so that I can get new notifications after lastTimestamp value
        var lastRequest = await  _context.NotificationRequests.LastOrDefaultAsync();
        if (lastRequest == null) return await _context.Notifications.ToListAsync();

        var notifications = _context.Notifications;
        
        var lastTimestamp = lastRequest.Timestamp;
        return await notifications
            .Where(n => n.CreatedAt > lastTimestamp)
            .ToListAsync();
       
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    //very ugly code, NotificationRequests should have it's own repository
    public async Task AddAsync(NotificationRequest request)
    {
        await _context.NotificationRequests.AddAsync(request);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }



    public void UpdateAsync(Notification notification)
    {
         _context.Notifications.Update(notification);
    }
}