using ArtPlatform.API.Extensions;
using ArtPlatform.API.Models.PortfolioModel;
using ArtPlatform.API.Models.SellerService;
using ArtPlatform.API.Services.Storage;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerServiceController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorage _storage;

    public SellerServiceController(ApplicationDbContext dbContext, IStorage storage)
    {
        _storage = storage;
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [Authorize("read:seller-service")]
    public async Task<IActionResult> GetSellerServices(int offset=0, int pageSize=10)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        
        if(seller==null)
            return BadRequest("Account is not a seller.");

        var sellerServices = await _dbContext.SellerServices.Include(x=>x.Reviews)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = sellerServices.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpGet]
    [Route("Count")]
    [Authorize("read:seller-service")]
    public async Task<IActionResult> GetSellerServicesCount()
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        
        if(seller==null)
            return BadRequest("Account is not a seller.");

        var sellerServices = await _dbContext.SellerServices.Include(x => x.Reviews).ToListAsync();
        var result = sellerServices.Count;
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize("write:seller-service")]
    public async Task<IActionResult> CreateSellerService([FromBody] SellerServiceCreateModel model)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        
        if(seller==null)
            return BadRequest("Account is not a seller.");

        var sellerService = new Database.Entities.SellerService()
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            SellerProfileId = seller.Id
        };
        sellerService = _dbContext.SellerServices.Add(sellerService).Entity;
        await _dbContext.SaveChangesAsync();
        var result = sellerService.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-service")]
    [Route("{sellerServiceId:int}")]
    public async Task<IActionResult> UpdateSellerService([FromBody] SellerServiceUpdateModel model, int sellerServiceId)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        
        if(seller==null)
            return BadRequest("Account is not a seller.");

        var sellerService = await _dbContext.SellerServices.FirstOrDefaultAsync(sellerService=>sellerService.Id==sellerServiceId);
        
        if(sellerService==null)
            return NotFound("Seller service not found.");

        sellerService.Name = model.Name;
        sellerService.Description = model.Description;
        sellerService.Price = model.Price;
        
        sellerService = _dbContext.SellerServices.Update(sellerService).Entity;
        await _dbContext.SaveChangesAsync();
        var result = sellerService.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:seller-service")]
    [Route("{sellerServiceId:int}")]
    public async Task<IActionResult> DeleteSellerService(int sellerServiceId)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles.FirstOrDefaultAsync(sellerProfile=>sellerProfile.UserId==userId);
        
        if(seller==null)
            return BadRequest("Account is not a seller.");

        var sellerService = await _dbContext.SellerServices.FirstOrDefaultAsync(sellerService=>sellerService.Id==sellerServiceId);
        
        if(sellerService==null)
            return NotFound("Seller service not found.");

        _dbContext.SellerServices.Remove(sellerService);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }   
    
    [HttpGet]
    [Route("{sellerServiceId:int}/Portfolio/")]
    public async Task<IActionResult> GetPortfolio(int sellerServiceId)
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
        var portfolio = await _dbContext.SellerProfilePortfolioPieces.Where(x=>x.SellerProfileId==existingSellerProfile.Id && x.SellerServiceId==sellerServiceId).ToListAsync();
        var result = portfolio.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }

    
    [HttpGet]
    [Authorize("read:seller-service")]
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
            .FirstAsync(x => x.SellerProfileId == existingSellerProfile.Id
                             && x.SellerServiceId == sellerServiceId && x.Id==portfolioId);
        var content = await _storage.DownloadImageAsync(portfolio.FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }
    
    [HttpPost]
    [Authorize("write:seller-service")]
    [Route("{sellerServiceId:int}/Portfolio")]
    public async Task<IActionResult> AddPortfolio(IFormFile file, int sellerServiceId)
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
            FileReference = url,
            SellerServiceId = sellerServiceId
        };
        portfolio.SellerProfileId = existingSellerProfile.Id;
        _dbContext.SellerProfilePortfolioPieces.Add(portfolio);
        await _dbContext.SaveChangesAsync();
        var result = portfolio.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:seller-service")]
    [Route("{sellerServiceId:int}/Portfolio/{portfolioId:int}")]
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
