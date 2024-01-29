using System.Security.Claims;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using comissions.app.api.Services.Payment;
using comissions.app.database;
using comissions.app.database.Entities;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Middleware;


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

            var user = await dbContext.Users.Include(x=>x.UserSellerProfile).FirstOrDefaultAsync(x=>x.Id==userId);

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

            if (user.Suspended)
            {
                if (user.UnsuspendDate < DateTime.UtcNow)
                {
                    user.Suspended = false;
                    user.SuspendedDate = null;
                    user.UnsuspendDate = null;
                    user.SuspendedReason = null;
                    user.SuspendAdminId = null;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var suspendDate = user.SuspendedDate.Value.ToString("MM/dd/yyyy");
                    var unsuspendDate = user.UnsuspendDate.Value.ToString("MM/dd/yyyy");
                    await context.Response.WriteAsync($"Suspended on {suspendDate} until {unsuspendDate} for {user.SuspendedReason} by {user.SuspendAdminId}.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            if (user.Banned)
            {
                if (user.UnsuspendDate < DateTime.UtcNow)
                {
                    user.Banned = false;
                    user.BannedDate = null;
                    user.BannedDate = null;
                    user.BannedReason = null;
                    user.BanAdminId = null;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var suspendDate = user.BannedDate.Value.ToString("MM/dd/yyyy");
                    var unsuspendDate = user.UnbanDate.Value.ToString("MM/dd/yyyy");
                    await context.Response.WriteAsync($"Banned on {suspendDate} until {unsuspendDate} for {user.BannedReason} by {user.BanAdminId}.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            if (user.UserSellerProfile != null && user.UserSellerProfile.Suspended)
            {
                if (user.UserSellerProfile.UnsuspendDate < DateTime.UtcNow)
                {
                    user.UserSellerProfile.Suspended = false;
                    user.UserSellerProfile.SuspendedDate = null;
                    user.UserSellerProfile.UnsuspendDate = null;
                    user.UserSellerProfile.SuspendedReason = null;
                    user.UserSellerProfile.SuspendAdminId = null;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var suspendDate = user.UserSellerProfile.SuspendedDate.Value.ToString("MM/dd/yyyy");
                    var unsuspendDate = user.UserSellerProfile.UnsuspendDate.Value.ToString("MM/dd/yyyy");
                    await context.Response.WriteAsync($"Banned on {suspendDate} until {unsuspendDate} for {user.UserSellerProfile.SuspendedReason} by {user.UserSellerProfile.SuspendAdminId}.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
        }

        await _next(context);
    }
}