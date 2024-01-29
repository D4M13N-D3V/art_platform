using comissions.app.api.Extensions;
using ArtPlatform.Database;
using comissions.app.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminUsersController:ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AdminUsersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsers(string search="", int offset = 0, int pageSize = 10)
    {
        var users = await _dbContext.Users
            .Where(x=>x.DisplayName.Contains(search) || x.Email.Contains(search))
            .Skip(offset).Take(pageSize).ToListAsync();
        return Ok(users);
    }
    
    [HttpGet("Count")]
    public async Task<IActionResult> GetUsersCount(string search="")
    {
        var result = await _dbContext.Users
            .Where(x=>x.DisplayName.Contains(search) || x.Email.Contains(search))
            .CountAsync();
        return Ok(result);
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);

        if (user == null)
            return NotFound("User not found.");
        
        return Ok(user);
    }
    
    [HttpGet("{userId}/Orders")]
    public async Task<IActionResult> GetUserOrders(string userId)
    {
        var user = await _dbContext.Users.Include(x=>x.Orders).FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound("User not found.");
        
        return Ok(user.Orders);
    }
    
    [HttpPut("{userId}/Suspend")]
    public async Task<IActionResult> SuspendUser(string userId, [FromQuery]string reason, [FromQuery]int days)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound("User not found.");
        
        user.Suspended = true;
        user.SuspendedDate = DateTime.UtcNow;
        user.SuspendedReason = reason;
        user.SuspendAdminId = User.GetUserId();
        user.UnsuspendDate = DateTime.UtcNow.AddDays(days);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/Unsuspend")]
    public async Task<IActionResult> UnsuspendUser(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound("User not found.");
        
        user.Suspended = false;
        user.SuspendedDate = null;
        user.SuspendedReason = null;
        user.SuspendAdminId = null;
        user.UnsuspendDate = null;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/Ban")]
    public async Task<IActionResult> BanUser(string userId, [FromQuery]string reason, [FromQuery]int days)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound("User not found.");
        
        user.Banned = true;
        user.BannedDate = DateTime.UtcNow;
        user.BannedReason = reason;
        user.BanAdminId = User.GetUserId();
        user.UnbanDate = DateTime.UtcNow.AddDays(days);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/Unban")]
    public async Task<IActionResult> UnbanUser(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound("User not found.");
        
        user.Banned = false;
        user.BannedDate = null;
        user.BannedReason = null;
        user.BanAdminId = null;
        user.UnbanDate = null;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/SetDisplayName")]
    public async Task<IActionResult> SetDisplayName(string userId, [FromBody]string displayName)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound("User not found.");
        
        user.DisplayName = displayName;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{userId}/SetBiography")]
    public async Task<IActionResult> SetBiography(string userId, [FromBody]string biography)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId);
        
        if (user == null)
            return NotFound("User not found.");
        
        user.Biography = biography;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
}