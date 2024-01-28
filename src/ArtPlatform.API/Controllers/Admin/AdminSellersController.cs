using ArtPlatform.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminSellersController
{
    private readonly ApplicationDbContext _dbContext;

    public AdminSellersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public Task<IActionResult> GetSellers(string search="", int offset = 0, int pageSize = 10)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("Count")]
    public Task<IActionResult> GetSellersCount(string search="")
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{sellerId:int}")]
    public Task<IActionResult> GetSeller(int sellerId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{sellerId:int}/Orders")]
    public Task<IActionResult> GetSellerOrders(int sellerId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{sellerId:int}/Suspend")]
    public Task<IActionResult> SuspendSeller(int sellerId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{sellerId:int}/Unsuspend")]
    public Task<IActionResult> UnsuspendSeller(int sellerId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{sellerId:int}/Terminate")]
    public Task<IActionResult> TerminateSeller(int sellerId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{sellerId:int}/SetBiography")]
    public Task<IActionResult> SetBiography(string userId, [FromBody]string biography)
    {
        throw new NotImplementedException();
    }
}