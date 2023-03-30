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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

//limiting the IP address request max 20 times in 1 minute
app.UseMiddleware<RateLimitingMiddleware>(new MemoryCache(new MemoryCacheOptions()), 20, TimeSpan.FromSeconds(60));
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PreDb.PrepPopulation(app);
app.Run();
