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

    public async Task<List<RatingNotification>> GetAllAsync()
    {
        return await _context.RatingNotifications.ToListAsync();
    }

    public async Task<List<RatingNotification>> GetNewAsync(DateTime lastAccessTime)
    {
        return await _context.RatingNotifications
            .Where(n => n.CreatedAt > lastAccessTime)
            .ToListAsync();
    }

    public async Task AddAsync(RatingNotification ratingNotification)
    {
        await _context.RatingNotifications.AddAsync(ratingNotification);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}