using System.Security.Claims;
using ArtPlatform.API.Services.Payment;
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

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, IPaymentService paymentService)
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
                    DisplayName = context.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Name)?.Value ?? "Anonymous", 
                    Biography = string.Empty,
                    Email = context.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Email)?.Value ?? string.Empty,
                };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
            else
            {   
                user.Email= context.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Email)?.Value ?? string.Empty;
                dbContext.Users.Update(user);
                        await dbContext.SaveChangesAsync();
            }
        }

        await _next(context);
    }
}