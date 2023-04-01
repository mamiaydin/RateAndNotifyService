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

    public async Task<List<Rating>> GetAllAsync()
    {
        var notifications = await _context.RatingNotifications.ToListAsync();
        return notifications;
    }

    public async Task<List<Rating>> GetNewAsync()
    {
        //get lastTimestamp value from request table so that I can get new notifications after lastTimestamp value
        var lastRequest = await  _context.NotificationRequests.LastOrDefaultAsync();
        if (lastRequest == null) return await _context.RatingNotifications.ToListAsync();
       
        var lastTimestamp = lastRequest.Timestamp;
        return await _context.RatingNotifications
            .Where(n => n.CreatedAt > lastTimestamp)
            .ToListAsync();
       
    }

    public async Task AddAsync(Rating rating)
    {
        await _context.RatingNotifications.AddAsync(rating);
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
}