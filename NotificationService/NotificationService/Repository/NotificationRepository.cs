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
        return await _context.RatingNotifications.ToListAsync();
    }

    public async Task<List<Rating>> GetNewAsync(DateTime lastAccessTime)
    {
        return await _context.RatingNotifications
            .Where(n => n.CreatedAt > lastAccessTime)
            .ToListAsync();
    }

    public async Task AddAsync(Rating rating)
    {
        await _context.RatingNotifications.AddAsync(rating);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}