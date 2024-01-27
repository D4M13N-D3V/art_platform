using ArtPlatform.API.Extensions;
using ArtPlatform.API.Models.PortfolioModel;
using ArtPlatform.API.Models.SellerProfile;
using ArtPlatform.API.Services.Storage;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerProfileController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorage _storage;


    public SellerProfileController(ApplicationDbContext dbContext, IStorage storage)
    {
        _storage = storage;
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [Authorize("read:seller-profile")]
    public async Task<IActionResult> GetSellerProfile()
    {
        var userId = User.GetUserId();
        var sellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if(sellerProfile==null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest("Account has requested to be a seller and not been approved yet.");
            return Unauthorized("Account is not a seller.");
        }
        var result = sellerProfile.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-profile")]
    public async Task<IActionResult> UpdateSellerProfile(SellerProfileModel model)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest("Account has requested to be a seller and not been approved yet.");
            return Unauthorized("Account is not a seller.");
        }
        var updatedSellerProfile = model.ToModel(existingSellerProfile);
        updatedSellerProfile = _dbContext.UserSellerProfiles.Update(updatedSellerProfile).Entity;
        await _dbContext.SaveChangesAsync();
        var result = updatedSellerProfile.ToModel();
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize("write:seller-profile")]
    public async Task<IActionResult> RequestSellerProfile(SellerProfileModel model)
    {
        var userId = User.GetUserId();
        
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile != null)
        {
            return Unauthorized("Account is already a seller.");
        }
        
        var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId);
        if(sellerProfileRequest!=null)
            return BadRequest("Account has already requested to be a seller.");
        
        sellerProfileRequest = new SellerProfileRequest()
        {
            Accepted = false,
            RequestDate = DateTime.UtcNow,
            UserId = userId
        };
        _dbContext.SellerProfileRequests.Add(sellerProfileRequest);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    [HttpGet]
    [Authorize("read:seller-profile")]
    [Route("{sellerServiceId:int}/Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> GetPortfolio(int sellerServiceId, int portfolioId)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest("Account has requested to be a seller and not been approved yet.");
            return Unauthorized("Account is not a seller.");
        }

        var portfolio = await _dbContext.SellerProfilePortfolioPieces
            .FirstAsync(x => x.SellerProfileId == existingSellerProfile.Id && x.Id==portfolioId);
        var content = await _storage.DownloadImageAsync(portfolio.FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }

    [HttpGet]
    [Route("Portfolio")]
    [Authorize("read:seller-profile")]
    public async Task<IActionResult> GetPortfolio()
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest("Account has requested to be a seller and not been approved yet.");
            return Unauthorized("Account is not a seller.");
        }
        var portfolio = await _dbContext.SellerProfilePortfolioPieces.Where(x=>x.SellerProfileId==existingSellerProfile.Id).ToListAsync();
        var result = portfolio.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpPost]
    [Route("Portfolio")]
    [Authorize("write:seller-profile")]
    public async Task<IActionResult> AddPortfolio(IFormFile file)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest("Account has requested to be a seller and not been approved yet.");
            return Unauthorized("Account is not a seller.");
        }

        var url = await _storage.UploadImageAsync(file, Guid.NewGuid().ToString());
        var portfolio = new SellerProfilePortfolioPiece()
        {
            SellerProfileId = existingSellerProfile.Id,
            FileReference = url
        };
        portfolio.SellerProfileId = existingSellerProfile.Id;
        _dbContext.SellerProfilePortfolioPieces.Add(portfolio);
        await _dbContext.SaveChangesAsync();
        var result = portfolio.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:seller-profile")]
    [Route("Portfolio/{portfolioId:int}")]
    public async Task<IActionResult> DeletePortfolio(int portfolioId)
    {
        var userId = User.GetUserId();
        var existingSellerProfile = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        if (existingSellerProfile == null)
        {
            var sellerProfileRequest = await _dbContext.SellerProfileRequests.FirstOrDefaultAsync(request=>request.UserId==userId && request.Accepted==false);
            if(sellerProfileRequest!=null)
                return BadRequest("Account has requested to be a seller and not been approved yet.");
            return Unauthorized("Account is not a seller.");
        }
        var portfolio = await _dbContext.SellerProfilePortfolioPieces.FirstOrDefaultAsync(x=>x.Id==portfolioId);
        if(portfolio==null)
            return NotFound("Portfolio piece not found.");
        if(portfolio.SellerProfileId!=existingSellerProfile.Id)
            return BadRequest("Portfolio piece does not belong to this seller.");
        _dbContext.SellerProfilePortfolioPieces.Remove(portfolio);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}