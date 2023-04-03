using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RatingService;
using RatingService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    //swagger initializations for development
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseMiddleware<RateLimitingMiddleware>(new MemoryCache(new MemoryCacheOptions()), 20, TimeSpan.FromSeconds(60));
}
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//feed database
PreDb.PrepPopulation(app);
app.Run();
