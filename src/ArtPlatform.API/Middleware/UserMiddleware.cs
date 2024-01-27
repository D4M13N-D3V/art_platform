using System.Security.Claims;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;

namespace ArtPlatform.API.Middleware;


public class UserMiddleware
{
    private readonly RequestDelegate _next;

    public UserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var user = await dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                user = new User
                {
                    Id = userId, 
                    DisplayName = context.User.Identity.Name ?? "Anonymous", 
                    Biography = string.Empty,
                    Email = context.User.Claims.FirstOrDefault(x=>x.Type=="email")?.Value ?? string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    AddressCountry = string.Empty,
                    AddressCity = string.Empty,
                    AddressStreet = string.Empty,
                    AddressHouseNumber = string.Empty,
                    AddressPostalCode = string.Empty,
                    AddressRegion = string.Empty
                };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
        }

        await _next(context);
    }
}