using AutoMapper;
using RatingService.Dtos;
using RatingService.Models;

namespace RatingService.Profiles;

public class RatingsProfile : Profile
{
    public RatingsProfile()
    { 
        CreateMap<Rating, RatingReadDto>();
        CreateMap<RatingCreateDto, Rating>();
        CreateMap<Rating, NotificationModel>();
    }
}