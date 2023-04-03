using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RatingService;
using RatingService.Controllers;
using RatingService.Data;
using RatingService.Dtos;
using RatingService.Models;

public class RatingControllerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IRatingRepository> _ratingRepositoryMock;
    private readonly RatingController _controller;

    public RatingControllerTests()
    {
        // Configure AutoMapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RatingCreateDto, Rating>();
            cfg.CreateMap<Rating, RatingReadDto>();
            cfg.CreateMap<Rating, NotificationModel>();
        });
        _mapper = configuration.CreateMapper();

        // Mock dependencies
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _ratingRepositoryMock = new Mock<IRatingRepository>();
        // Create controller instance
        _controller = new RatingController(
            _ratingRepositoryMock.Object,
            _mapper,
            _serviceRepositoryMock.Object
            );
    }

    // Test method to check if a valid rating create DTO returns a CreatedAtRouteResult
    [Fact]
    public async Task SubmitRating_ValidDto_ReturnsCreatedAtRouteResult()
    {
        // Arrange
        var ratingCreateDto = new RatingCreateDto
        {
            ServiceId = 1,
            Score = 4
        };
        var service = new Service { Id = 1 };
        _serviceRepositoryMock.Setup(repo => repo.GetServiceById(1)).Returns(service);
        _ratingRepositoryMock.Setup(repo => repo.GetAverageRatingOfService(1)).Returns(4);
        var notificationProducer = new NotificationProducer("notifications_queue");
        notificationProducer.PublishNotification(It.IsAny<NotificationModel>());

        // Act
        var result = await _controller.SubmitRating(ratingCreateDto);

        // Assert
        var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var ratingReadDto = Assert.IsType<RatingReadDto>(createdAtRouteResult.Value);
        Assert.Equal(service.Id, ratingReadDto.ServiceId);
        Assert.Equal(ratingCreateDto.Score, ratingReadDto.Score);
        Assert.Equal(4, ratingReadDto.AvgScore);
    }
    

    // Test method to check if SubmitRating returns a NotFoundObjectResult when the service is not found
    [Fact]
    public async Task SubmitRating_ServiceNotFound_ReturnsNotFoundObjectResult()
    {
        // Arrange
        var ratingCreateDto = new RatingCreateDto { ServiceId = 1 };
        _serviceRepositoryMock.Setup(repo => repo.GetServiceById(1)).Returns((Service)null);

        // Act
        var result = await _controller.SubmitRating(ratingCreateDto);

        // Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Service not found", notFoundObjectResult.Value);
    }
}