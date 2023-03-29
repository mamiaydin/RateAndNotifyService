using RatingService.Models;

namespace RatingService.Data;

public static class PreDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
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