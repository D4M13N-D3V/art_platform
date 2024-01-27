using System.Security.Claims;
using ArtPlatform.API.Extensions;
using ArtPlatform.API.Models.User;
using ArtPlatform.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

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