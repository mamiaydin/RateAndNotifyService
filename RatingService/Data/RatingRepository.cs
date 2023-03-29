using RatingService.Models;

namespace RatingService.Data;

public class RatingRepository:IRatingRepository
{
    private readonly AppDbContext _context;

    public RatingRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public void Save()
    {
        _context.SaveChangesAsync();
    }

    public IEnumerable<Rating> GetAllRatings()
    {
        return _context.Ratings.ToList();
    }

    public Rating GetRatingById(int id)
    {
        return _context.Ratings.FirstOrDefault(x => x.Id == id);
    }

    public void CreateRating(Rating rating)
    {
        if (rating == null) throw new ArgumentNullException(nameof(rating));
        
        _context.Ratings.Add(rating);
    }
}