using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;
using NotificationService.Repository;

namespace NotificationService.Test;

public class NotificationRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;
    
    public NotificationRepositoryTests()
    {
        // Set up an in-memory database for testing
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }

    
    //This is a test method that tests the GetNewNotificationsAsync method of the NotificationRepository class. Do the followings:
    [Fact]
    public async Task GetNewNotificationsAsync_ShouldReturnNewNotifications()
    {
        // Arrange
        var context = new AppDbContext(_options);
        var repository = new NotificationRepository(context);

        var notifications = new List<Notification>
        {
            new() {Score = 4, ServiceId = 1, CreatedAt = DateTime.UtcNow},
            new() {Score = 3, ServiceId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(1)},
            new() {Score = 5, ServiceId = 2, CreatedAt = DateTime.UtcNow.AddMinutes(5)}
        };
        context.Notifications.AddRange(notifications);//Creates a list of notifications and adds them to the Notifications table in the database using the AppDbContext instance.
        await context.SaveChangesAsync();

        var lastRequest = new NotificationRequest {Guid = new Guid(), Timestamp = DateTime.UtcNow};//Creates a NotificationRequest instance and adds it to the NotificationRequests table in the database using the AppDbContext instance.
        context.NotificationRequests.Add(lastRequest);

        await context.SaveChangesAsync();


        // Act
        var result = await repository.GetNewNotificationsAsync();//Calls the GetNewNotificationsAsync method of the NotificationRepository instance.
        
        //Asserts that the returned list of notifications has the expected count and that the properties of the first notification in the list are equal to the expected values.
        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(notifications[1].Score, result[0].Score);
        Assert.Equal(notifications[1].ServiceId, result[0].ServiceId);
        Assert.Equal(notifications[1].CreatedAt, result[0].CreatedAt);
        Assert.Equal(notifications[1].CreatedIp, result[0].CreatedIp);
        Assert.Equal(notifications[1].AvgScore, result[0].AvgScore);
    }
    
}