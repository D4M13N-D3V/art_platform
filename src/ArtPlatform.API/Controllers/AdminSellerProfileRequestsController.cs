using ArtPlatform.API.Models.SellerProfileRequest;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/[controller]")]
public class AdminSellerProfileRequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public AdminSellerProfileRequestsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Gets a list of all of the requests from users to become a seller.
    /// </summary>
    /// <param name="offset"> The offset to start at.</param>
    /// <param name="pageSize"> The amount of records to return.</param>
    /// <returns>A list of seller profile requests</returns>
    [HttpGet]
    [Authorize("read:seller-profile-request")]
    [Route("SellerRequests")]
    public async Task<IActionResult> GetSellerRequests(int offset = 0, int pageSize = 10)
    {
        var requests = _dbContext.SellerProfileRequests.Skip(offset).Take(pageSize).ToList();
        var result = requests.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    /// <summary>
    /// Gets the amount of requests there are from users to become a seller.
    /// </summary>
    /// <returns>The number of requests.</returns>
    [HttpGet]
    [Authorize("read:seller-profile-request")]
    [Route("SellerRequests/Count")]
    public async Task<IActionResult> GetSellerRequestsCount()
    {
        var result = _dbContext.SellerProfileRequests.Count();
        return Ok(result);
    }
    
    /// <summary>
    /// Accepts a request to become a seller from a user.
    /// </summary>
    /// <param name="userId">The ID of the user to accept the request for.</param>
    /// <returns>The new seller profile.</returns>
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