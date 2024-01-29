using System.Security.Claims;
using comissions.app.api.Extensions;
using ArtPlatform.Database;
using comissions.app.api.Models.User;
using comissions.app.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public UserController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [Authorize("read:user")]
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var userId = User.GetUserId();
        var user = await _dbContext.Users.FirstAsync(user=>user.Id==userId);
        var result = user.ToModel();
        return Ok(result);
    }
    
    [Authorize("write:user")]
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserInfoUpdateModel model)
    {
        var userId = User.GetUserId();
        var existingUser = await _dbContext.Users.FirstAsync(user=>user.Id==userId);
        var updatedUser = model.ToEntity(existingUser);
        updatedUser = _dbContext.Users.Update(updatedUser).Entity;
        await _dbContext.SaveChangesAsync();
        var result = updatedUser.ToModel();
        return Ok(result);
    }
}