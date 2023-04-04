using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatingService.Data;
using RatingService.Dtos;
using RatingService.Models;

namespace RatingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IMapper _mapper;

    public RatingController(IRatingRepository repository,IMapper mapper, IServiceRepository serviceRepository)
    {
        _ratingRepository = repository;
        _mapper = mapper;
        _serviceRepository = serviceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RatingReadDto>>> GetRatings()
    {
        var ratingItems = _ratingRepository.GetAllRatings();
        var ratings = _mapper.Map<IEnumerable<RatingReadDto>>(ratingItems);
        return Ok(ratings);
    }
    
    [HttpGet("{id:int}" , Name = "GetRatingById")]
    public ActionResult<RatingReadDto> GetRatingById(int id)
    {
        var ratingItem = _ratingRepository.GetRatingById(id);
        if (ratingItem == null) return NotFound("Rating Not found");
        
        var ratings = _mapper.Map<RatingReadDto>(ratingItem);
        return Ok(ratings);
    }

    // This is the HTTP POST endpoint for submitting a new rating for a service
    [HttpPost]
    public async Task<ActionResult<RatingReadDto>> SubmitRating(RatingCreateDto ratingCreateDto)
    {
        // Get the service by ID from the service repository
        var service = _serviceRepository.GetServiceById(ratingCreateDto.ServiceId);

        // If the service is not found, return a 404 error
        if (service == null) return NotFound("Service not found");

        // If the model state is not valid, return an Unprocessable Entity error
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        // Map the rating create DTO to a rating object
        var rating = _mapper.Map<Rating>(ratingCreateDto);

        // Set the IP address of the client that submitted the rating
        if (HttpContext != null) rating.CreatedIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Save the new rating to the rating repository
        await _ratingRepository.CreateRatingAsync(rating);

        // Get the average rating score for the service
        var avgScore = _ratingRepository.GetAverageRatingOfService(service.Id);

        // Push the newly created rating to the RabbitMQ queue named "notifications_queue"
        var producer = new NotificationProducer("notifications_queue");
        try
        {
            // Map the rating to a notification model
            var notificationModel = _mapper.Map<NotificationModel>(rating);

            // Set the average score for the notification model
            notificationModel.AvgScore = avgScore;

            // Publish the notification model to the queue
            producer.PublishNotification(notificationModel);
        }
        catch (Exception ex)
        {
            //todo Handle return result
            
            // If an error occurs while publishing the notification, log the error message to the console
            Console.WriteLine($"{ex} : Can't publish notification, check your rabbitmq server!");
        }
        finally
        {
            // Dispose of the RabbitMQ producer
            producer.Dispose();
        }

        // Map the newly created rating to a rating read DTO
        var ratingReadDto = _mapper.Map<RatingReadDto>(rating);

        // Set the average score for the rating read DTO
        ratingReadDto.AvgScore = avgScore;

        // Return a 201 Created response with the location of the newly created rating in the response headers
        return CreatedAtRoute(nameof(GetRatingById), new {ratingReadDto.Id}, ratingReadDto);
    }

}