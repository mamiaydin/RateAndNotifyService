using RatingService.Models;

namespace RatingService.Data;

public interface IRatingRepository
{
    void Save();

    IEnumerable<Rating> GetAllRatings();
    Rating GetRatingById(int id);
    void SubmitRating(Rating rating);
}