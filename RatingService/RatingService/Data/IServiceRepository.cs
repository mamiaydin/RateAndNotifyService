using RatingService.Models;

namespace RatingService.Data;

public interface IServiceRepository
{
    void Save();

    Service GetServiceById(int id);
}