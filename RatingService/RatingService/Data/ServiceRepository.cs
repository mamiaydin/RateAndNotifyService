using RatingService.Models;

namespace RatingService.Data;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;
    
    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }
    public void Save()
    {
        _context.SaveChangesAsync();
    }

    public Service GetServiceById(int id)
    {
        return _context.Service.FirstOrDefault(x => x.Id == id);
    }
}