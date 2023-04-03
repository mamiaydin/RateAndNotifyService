using AutoMapper;
using RatingService.Models;

namespace RatingService.Data;

public class RatingRepository:IRatingRepository
{
    private readonly AppDbContext _context;

    public RatingRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public IEnumerable<Rating> GetAllRatings()
    {
        return _context.Ratings.ToList();
    }

    public Rating GetRatingById(int id)
    {
        return _context.Ratings.FirstOrDefault(x => x.Id == id);
    }

    public async Task CreateRatingAsync(Rating rating)
    {
        if (rating == null) throw new ArgumentNullException(nameof(rating));
        
        await _context.Ratings.AddAsync(rating);
        //save immediately
        await Save();

    }

    public double GetAverageRatingOfService(int serviceId)
    {
        var ratingScores = _context.Ratings.Where(x => x.ServiceId == serviceId);
        var averageRating = ratingScores.Average(r => r.Score);
        return averageRating;
    }
}