using ArtPlatform.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminUsersController
{
    private readonly ApplicationDbContext _dbContext;

    public AdminUsersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public Task<IActionResult> GetUsers(string search="", int offset = 0, int pageSize = 10)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("Count")]
    public Task<IActionResult> GetUsersCount(string search="")
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{userId}")]
    public Task<IActionResult> GetUser(string userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{userId}/Orders")]
    public Task<IActionResult> GetUserOrders(string userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{userId}/Suspend")]
    public Task<IActionResult> SuspendUser(string userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{userId}/Unsuspend")]
    public Task<IActionResult> UnsuspendUser(string userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{userId}/Terminate")]
    public Task<IActionResult> TerminateUser(string userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{userId}/SetDisplayName")]
    public Task<IActionResult> SetDisplayName(string userId, [FromBody]string displayName)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{userId}/SetBiography")]
    public Task<IActionResult> SetBiography(string userId, [FromBody]string biography)
    {
        throw new NotImplementedException();
    }
    
}