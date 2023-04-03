using RatingService.Models;

namespace RatingService.Data;

public interface IRatingRepository
{
    Task Save();

    IEnumerable<Rating> GetAllRatings();
    Rating GetRatingById(int id);
    Task CreateRatingAsync(Rating rating);
    double GetAverageRatingOfService(int serviceId);
}