using Microsoft.EntityFrameworkCore;
using RatingService.Models;

namespace RatingService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Service> Service { get; set; }
}

