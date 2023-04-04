# Service Provider Ratings and Notifications

## How to Run
- download docker and docker-compose
- clone this repository
- run docker
- set working directory to RateAndNotifyService `cd RateAndNotifyService`
- run `docker-compose up [-d]` without -d command we can see that Rabbitmq, RatingService and NotifiyService will open respectively.
    - Notification Service will run on *http://localhost:5002/swagger/index.html* 

    - Rating Service will run on *http://localhost:5001/swagger/index.html* 

    - RabbitMQ management UI *http://localhost:15672*  Username: `admin`  Password: `password`

## Notes

- I used RabbitMQ to allow services to communicate with each other. The first service, called the **RatingService** as a producer, sends messages to a RabbitMQ exchange, while the second service, called the **NotificationService** , subscribes to the exchange to receive messages.

- I used **Microsoft.EntityFrameworkCore** package, implement DbContext and used Repository Design Pattern for both services. 

- Services working independently. Services are not aware of each other.

- I include OpenAPI(Swagger) for both services.

- I used xUnit and Moq libraries for unit tests. I kept short unit tests for simplicity.

## Tests
`cd RatingService` or `cd NotificationService`
run `dotnet test`


## Rating Service
**Task:** Allows users to submit a rating for a service provider and fetch the
average rating for a specific service provider. This service will persist the rating data
and notify the notification service when a new rating is submitted.

- I have **Service** and **Rating** models. **Rating** Model stands for submiting rating to a spesific **Service**. So we could bind any service to RatingService than we can rate it.

- I seperate reading and creating dto Models of Rating Model for future implementations(ex. CQRS if neccesity) it is good to distinguish read and write parts of application. For the mapping thhese dto Models I used **AutoMapper.Extensions.Microsoft.DependencyInjection** package.

### RatingsController `HttpPost` method for the task: *Allows users to submit a rating for a service provider and fetch the
average rating for a specific service provider.*

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

### Sample SubmitRating Posting
![image](https://user-images.githubusercontent.com/43674003/229383700-4fb71ea4-6961-4c5d-ba36-48f107b946e4.png)

### Seeding Database

```
// This is a static class for pre-populating the database with initial data
public static class PreDb 
{
    // This method is called to prepare and populate the database
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();// Create a service scope to access the database context
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());// Call SeedData method to seed the database with initial data
    }

    private static void SeedData(AppDbContext context)
    {
        if (!context.Ratings.Any())
        {
            Console.WriteLine("Seeding data");
            context.Service.AddRange(
                new Service { Name = "Service Mars" },
                new Service { Name = "Service Venus" });

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("We already have data");
        }
    }
}
```

in Program.cs `PreDb.PrepPopulation(app);`

### Rate Limiting Middleware

Since I didn't implement Authentication there is no User. But I am limiting the Ip address if more than 20 request incoming in 1 minute.

```
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly int _maxRequests;
    private readonly TimeSpan _interval;

    public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, int maxRequests, TimeSpan interval)
    {
        _next = next;
        _cache = cache;
        _maxRequests = maxRequests;
        _interval = interval;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get the IP address of the remote client
        var remoteIpAddress = context.Connection.RemoteIpAddress.ToString();

        //if some reason remoteIpAddress can't read from context go next middleware
        if (remoteIpAddress == null) await _next(context);
        
        // Generate a cache key for the IP address and interval
        var cacheKey = $"{remoteIpAddress}-{_interval.TotalSeconds}";

        // Get the current number of requests for this IP address and interval
        var requestCount = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SetAbsoluteExpiration(_interval);
            return 0;
        });

        // Check if the request count is over the limit
        if (requestCount >= _maxRequests)
        {
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsync("You have exceeded the maximum number of requests. After 1 minute, Please try again later.");
            return;
        }

        // Increment the request count and update the cache
        _cache.Set(cacheKey, requestCount + 1, _interval);

        // If the request count is under the limit, call the next middleware component in the pipeline
        await _next(context);
    }
```

## Notification Service

**Task:** Allows clients to fetch a list of new notifications that have been
submitted since the last time the endpoint was called.

I have `NotificationBackgroundService` that listens RabbitMQ server with NotificationListener class in background as a HostedService.

`builder.Services.AddHostedService<NotificationBackgroundService>();`

If there is available rabbitmq connection and there is notification 'notifications_queue' in rabbitmq queue list, all coming notifications inserting to database. 
- If there is connection check connection and reconnect to rabbitmq channel at 5 minutes intervals. 
- If there is no connection try to connect rabbitmq channel at 10 seconds intervals

#### NotificationController `HttpGet` method for the task: *fetch a list of new notifications that have been submitted since the last time the endpoint was called*

    public async Task<ActionResult<List<Notification>>> GetNewNotifications()
    {
        // Retrieve new notifications from the NotificationService
        var newNotifications = await _notificationService.GetNewNotificationsAsync();
        
        // Create a new request in the in-memory database to store the timestamp of when the notifications were retrieved
        var notificationRequest = new NotificationRequest
            {Guid = Guid.NewGuid(), NotificationCount = 0, Timestamp = DateTime.Now};
        await _notificationService.CreateNotificationRequestAsync(notificationRequest);
        
        // Return the list of new notifications to the client
        if (!newNotifications.Any()) return Ok("Notifications empty..");
        return Ok(newNotifications);
    }

`builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();`

I registering the implementation classes of the INotificationRepository and INotificationService interfaces with the dependency injection container.
AddScoped method is used to register the services with a scoped lifetime. This means that an instance of the service is created once per request within the scope.

### Sample newNotifications result
![image](https://user-images.githubusercontent.com/43674003/229384113-7cde3386-5f33-44b7-93b9-c152bc3c3352.png)



If there is no available connection `NotificationListener` retrying to listen rabbitmq server 10 seconds delay.
In this case for simplicty I used InMemoryDatabase. **Instead of InMemoryDatabase I suggest that using nosql like mongodb would be better for notification service. Since we don't care data consistency and relational data.**


## Maintainability:

- The project uses dependency injection to manage dependencies and improve maintainability.
- AutoMapper is used to map between DTOs and domain models, reducing the amount of boilerplate code needed for mapping.
- The use of interfaces and repositories allows for easy swapping of data sources and improves testability.
- The use of a DbContext and migrations allows for easy database schema changes and versioning.

#### Improvement

- Implementing SOLID principles and design patterns can improve maintainability by making the code more modular and easier to understand and modify.
- Using code analysis tools and following coding standards can improve code quality and maintainability.
- Implementing automated testing and continuous integration can improve maintainability by catching errors early and ensuring that changes don't break existing functionality.

## Reliability:

- The use of try-catch blocks and error handling ensures that errors are caught and handled appropriately.
- The use of RabbitMQ for notifications ensures that notifications are reliably delivered even if the application is down or restarted.

#### Improvement

- Implementing fault tolerance and redundancy can improve reliability by ensuring that the application can continue to function even if certain components fail.
- Implementing monitoring and logging can improve reliability by allowing for quick identification and resolution of issues.
- Implementing automated testing and continuous integration can improve reliability by catching errors early and ensuring that changes don't break existing functionality.

## Scalability:

- The use of RabbitMQ for notifications allows for easy scaling of the notification system by adding more consumers.
- The use of asynchronous methods and tasks allows for better performance and scalability by freeing up threads to handle other requests.

#### Improvement

- Implementing load balancing and horizontal scaling can improve scalability by allowing the application to handle more requests and traffic.
- Implementing caching and optimizing database queries can improve scalability by reducing the load on the database.
- Implementing asynchronous and event-driven architectures can improve scalability by allowing the application to handle more requests and events without blocking.

# Thank you for your time :)
