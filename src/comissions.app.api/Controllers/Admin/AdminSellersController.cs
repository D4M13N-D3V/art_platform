using comissions.app.api.Extensions;
using ArtPlatform.Database;
using comissions.app.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminSellersController:ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AdminSellersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSellers(string search="", int offset = 0, int pageSize = 10)
    {
        var sellers = await _dbContext.UserSellerProfiles.Include(x=>x.User)
            .Where(x=>x.User.DisplayName.Contains(search) || x.User.Email.Contains(search))
            .Skip(offset).Take(pageSize).ToListAsync();
        return Ok(sellers);
    }
    
    [HttpGet("Count")]
    public async Task<IActionResult> GetSellersCount(string search="")
    {
        var result = await _dbContext.UserSellerProfiles.Include(x=>x.User)
            .Where(x=>x.User.DisplayName.Contains(search) || x.User.Email.Contains(search))
            .CountAsync();
        return Ok(result);
    }
    
    [HttpGet("{sellerId:int}")]
    public async Task<IActionResult> GetSeller(int sellerId)
    {
        var seller = await _dbContext.UserSellerProfiles.Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);

        if (seller == null)
            return NotFound("Seller not found.");
        
        return Ok(seller);
    }
    
    [HttpGet("{sellerId:int}/Orders")]
    public async Task<IActionResult> GetSellerOrders(int sellerId)
    {
        var seller = _dbContext.UserSellerProfiles.Include(x=>x.User)
            .FirstOrDefault(x=>x.Id==sellerId);
        
        if (seller == null)
            return NotFound("Seller not found.");
        
        var orders = await _dbContext.SellerServiceOrders.Where(x=>x.SellerId==sellerId).ToListAsync();
        return Ok(orders);
    }
    
    [HttpPut("{sellerId:int}/Suspend")]
    public async Task<IActionResult> SuspendSeller(int sellerId, [FromQuery]string reason, [FromQuery]int days)
    {
        var seller = _dbContext.UserSellerProfiles.FirstOrDefault(x=>x.Id==sellerId);
        
        if (seller == null)
            return NotFound("Seller not found.");

        if (seller.Suspended)
            return BadRequest("Seller is already suspended.");
        
        seller.Suspended = true;
        seller.SuspendedDate = DateTime.UtcNow;
        seller.UnsuspendDate = DateTime.UtcNow.AddDays(days);
        seller.SuspendedReason = reason;
        seller.SuspendAdminId = User.GetUserId();
        _dbContext.UserSellerProfiles.Update(seller);
        
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{sellerId:int}/Unsuspend")]
    public async Task<IActionResult> UnsuspendSeller(int sellerId)
    {
        var seller = _dbContext.UserSellerProfiles.FirstOrDefault(x=>x.Id==sellerId);
        
        if (seller == null)
            return NotFound("Seller not found.");

        if (!seller.Suspended)
            return BadRequest("Seller is not suspended.");
        
        seller.Suspended = false;
        seller.SuspendedDate = null;
        seller.UnsuspendDate = null;
        seller.SuspendedReason = null;
        seller.SuspendAdminId = null;
        _dbContext.UserSellerProfiles.Update(seller);
        
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{sellerId:int}/Terminate")]
    public async Task<IActionResult> TerminateSeller(int sellerId)
    {
        var seller = _dbContext.UserSellerProfiles.FirstOrDefault(x=>x.Id==sellerId);
        
        if (seller == null)
            return NotFound("Seller not found.");

        if (!seller.Suspended)
            return BadRequest("Seller is not suspended.");

        _dbContext.UserSellerProfiles.Remove(seller);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{sellerId:int}/SetBiography")]
    public async Task<IActionResult> SetBiography(int sellerId, [FromBody]string biography)
    {
        var seller = _dbContext.UserSellerProfiles.FirstOrDefault(x=>x.Id==sellerId);
        
        if (seller == null)
            return NotFound("Seller not found.");

        if (!seller.Suspended)
            return BadRequest("Seller is not suspended.");

        seller.Biography = biography;
        _dbContext.UserSellerProfiles.Update(seller);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}