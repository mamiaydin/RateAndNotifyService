using RatingService.Models;

namespace RatingService.Data;

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