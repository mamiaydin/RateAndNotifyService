﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatingService.Data;
using RatingService.Dtos;
using RatingService.Models;

namespace RatingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IMapper _mapper;

    public RatingsController(IRatingRepository repository,IMapper mapper, IServiceRepository serviceRepository)
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
    
    [HttpPost]
    public async Task<ActionResult<RatingReadDto>> SubmitRating(RatingCreateDto ratingCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        
        
        var rating = _mapper.Map<Rating>(ratingCreateDto);
        rating.CreatedIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        _ratingRepository.CreateRating(rating);
        _ratingRepository.Save();
        
        var ratingReadDto = _mapper.Map<RatingReadDto>(rating);
        
        //push created Rating to the Rabbitmq Queue list that name is "notifications_queue"
        var producer = new NotificationProducer("notifications_queue");
        try
        {
            producer.PublishNotification(rating);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex} : Can't publish notification, check your rabbitmq server!");
        }
        finally
        {
            producer.Dispose();
        }
        
        //return Response Code 201
        return CreatedAtRoute(nameof(GetRatingById), new {ratingReadDto.Id}, ratingReadDto);
    }
    
    [HttpGet("avg/{serviceId:int}")]
    public async Task<ActionResult<RatingReadDto>> GetAverageRating(int serviceId)
    {
        var service = _serviceRepository.GetServiceById(serviceId);
        if (service == null) return NotFound("Service not found");
        
        var ratingScores = _ratingRepository.GetAllRatings().Where(x => x.ServiceId == serviceId).Select(x=>x.Score);
        if (!ratingScores.Any())
        {
            return Ok($"This service {service.Name} not rated yet!");
        }
        
        var avgRate = ratingScores.Average();
        return Ok($"Averege Rate of {service.Name} is: {avgRate}" );
        
    }
    
}