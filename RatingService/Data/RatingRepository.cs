using AutoMapper;
using RatingService.Models;

namespace RatingService.Data;

public class RatingRepository:IRatingRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RatingRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
       
        
        try
        {
            var ratingNotification = new RatingNotification();
            ratingNotification = _mapper.Map<RatingNotification>(rating);
            ratingNotification.RatingId = rating.Id;
            
            var queueName = "notifications_queue";
            var producer = new NotificationProducer(queueName);
            var notification = ratingNotification;
            producer.PublishNotification(notification);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex} : Can't publish notification, check your rabbitmq server!");
        }

    }
}