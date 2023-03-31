using Microsoft.EntityFrameworkCore;
using NotificationService;
using NotificationService.Data;
using NotificationService.Repository;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRatingNotificationService, RatingNotificationService>();

builder.Services.AddSingleton<IHostedService, NotificationBackgroundService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();