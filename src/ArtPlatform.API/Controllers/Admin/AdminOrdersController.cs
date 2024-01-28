using ArtPlatform.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminOrdersController
{
    private readonly ApplicationDbContext _dbContext;

    public AdminOrdersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public Task<IActionResult> GetOrders(string search="", int offset = 0, int pageSize = 10)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("Count")]
    public Task<IActionResult> GetOrdersCount(string search="")
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{orderId:int}")]
    public Task<IActionResult> GetOrder(int orderId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("{orderId:int}")]
    public Task<IActionResult> SendMessage(int orderId, [FromBody]string message)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{orderId:int}/Suspend")]
    public Task<IActionResult> SuspendOrder(int orderId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{orderId:int}/Unsuspend")]
    public Task<IActionResult> UnsuspendOrder(int orderId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{orderId:int}/Terminate")]
    public Task<IActionResult> TerminateOrder(int orderId)
    {
        throw new NotImplementedException();
    }
}