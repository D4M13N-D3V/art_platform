using ArtPlatform.API.Models.SellerProfileRequest;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminSellerProfileRequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public AdminSellerProfileRequestsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [Authorize("read:seller-profile-request")]
    [Route("SellerRequests")]
    public async Task<IActionResult> GetSellerRequests(int offset = 0, int pageSize = 10)
    {
        var requests = _dbContext.SellerProfileRequests.Skip(offset).Take(pageSize).ToList();
        var result = requests.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize("read:seller-profile-request")]
    [Route("SellerRequests/Count")]
    public async Task<IActionResult> GetSellerRequestsCount()
    {
        var result = _dbContext.SellerProfileRequests.Count();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-profile-request")]
    [Route("SellerRequests/{userId}")]
    public async Task<IActionResult> AcceptSellerRequest(string userId)
    {
        var request = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        
        if(request==null)
            return NotFound("No request for that user exists.");
        
        if (request.Accepted == true)
            return BadRequest("User is already a seller.");

        request.Accepted = true;
        request.AcceptedDate = DateTime.UtcNow;

        var newSellerProfile = new UserSellerProfile()
        {
            UserId = userId,
            AgeRestricted = false,
            Biography = string.Empty,
            SocialMediaLinks = new List<string>(){}
        };
        _dbContext.UserSellerProfiles.Add(newSellerProfile);
        request = _dbContext.SellerProfileRequests.Update(request).Entity;
        await _dbContext.SaveChangesAsync();
        var result = request.ToModel();
        return Ok(result);
    }
}