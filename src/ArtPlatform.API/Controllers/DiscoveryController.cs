using ArtPlatform.API.Models.Discovery;
using ArtPlatform.API.Models.SellerProfile;
using ArtPlatform.API.Models.SellerService;
using ArtPlatform.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscoveryController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public DiscoveryController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    [HttpGet]
    [Route("Sellers")]
    public async Task<IActionResult> GetSellers(string search="",int offset = 0, int pageSize = 10)
    {
        var sellers = await _dbContext.UserSellerProfiles
            .Where(x=>x.User.DisplayName.Contains(search))
            .Include(x=>x.User)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellers.Select(x=>x.ToDiscoveryModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/Count")]
    public async Task<IActionResult> GetSellersCount(string search="")
    {
        var result = await _dbContext.UserSellerProfiles
            .Where(x=>x.User.DisplayName.Contains(search))
            .Include(x=>x.User)
            .CountAsync();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}/Services")]
    public async Task<IActionResult> GetSellerServices(int sellerId, int offset = 0, int pageSize = 10)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound("Seller not found.");
        var sellerServices = await _dbContext.SellerServices
            .Include(x=>x.Reviews)
            .Where(x=>x.SellerProfileId==sellerId)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellerServices.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}/Services/Count")]
    public async Task<IActionResult> GetSellerServicesCount(int sellerId)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound("Seller not found.");
        var sellerServices = await _dbContext.SellerServices
            .Include(x=>x.Reviews)
            .Where(x=>x.SellerProfileId==sellerId)
            .ToListAsync();
        var result = sellerServices.Count;
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}/Services/{serviceId:int}/Reviews")]
    public async Task<IActionResult> GetSellerServiceReviews(int sellerId, int serviceId, int offset = 0, int pageSize = 10)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound("Seller not found.");
        var sellerService = await _dbContext.SellerServices
            .Include(x=>x.Reviews).ThenInclude(x=>x.Reviewer)
            .FirstOrDefaultAsync(x=>x.Id==serviceId);
        if(sellerService==null)
            return NotFound("Seller service not found.");
        var result = sellerService.Reviews.Select(x=> new DiscoveryReviewModel()
        {
            Rating = x.Rating,
            WriterDisplayName = x.Reviewer.DisplayName,
            WriterId = x.ReviewerId,
        }).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Sellers/{sellerId:int}/Services/{serviceId:int}/Reviews/Count")]
    public async Task<IActionResult> GetSellerServiceReviewsCount(int sellerId, int serviceId)
    {
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound("Seller not found.");
        var sellerService = await _dbContext.SellerServices
            .Include(x=>x.Reviews).ThenInclude(x=>x.Reviewer)
            .FirstOrDefaultAsync(x=>x.Id==serviceId);
        if(sellerService==null)
            return NotFound("Seller service not found.");
        var result = sellerService.Reviews.Count;
        return Ok(result);
    }
}