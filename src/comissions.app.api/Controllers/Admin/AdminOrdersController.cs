using comissions.app.api.Extensions;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.api.Controllers;

[ApiController]
[Authorize("admin")]
[Route("api/admin/[controller]")]
public class AdminOrdersController:ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AdminOrdersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrders(string search="", int offset = 0, int pageSize = 10)
    {
        var orders = _dbContext.SellerServiceOrders.Include(x=>x.Seller).ThenInclude(x=>x.User).Include(x=>x.Buyer)
            .Where(x=>x.Seller.User.DisplayName.Contains(search) 
                      || x.Seller.User.Email.Contains(search) 
                      || x.Buyer.DisplayName.Contains(search) 
                      || x.Buyer.Email.Contains(search))
            .Skip(offset).Take(pageSize).ToList();
        return Ok(orders);
    }
    
    [HttpGet("Count")]
    public async Task<IActionResult> GetOrdersCount(string search="")
    {
        var result = _dbContext.SellerServiceOrders.Include(x=>x.Seller).ThenInclude(x=>x.User).Include(x=>x.Buyer)
            .Where(x=>x.Seller.User.DisplayName.Contains(search) 
                      || x.Seller.User.Email.Contains(search) 
                      || x.Buyer.DisplayName.Contains(search) 
                      || x.Buyer.Email.Contains(search))
            .Count();
        return Ok(result);
    }
    
    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var order = await _dbContext.SellerServiceOrders.Include(x=>x.Seller).ThenInclude(x=>x.User).Include(x=>x.Buyer)
            .FirstOrDefaultAsync(x=>x.Id==orderId);

        if (order == null)
            return NotFound("Order not found.");
        
        return Ok(order);
    }
    
    [HttpPost("{orderId:int}")]
    public async Task<IActionResult> SendMessage(int orderId, [FromBody]string message)
    {
        var order = await _dbContext.SellerServiceOrders.Include(x=>x.Seller).ThenInclude(x=>x.User).Include(x=>x.Buyer)
            .FirstOrDefaultAsync(x=>x.Id==orderId);

        if (order == null)
            return NotFound("Order not found.");
        
        order.Messages.Add(new SellerServiceOrderMessage()
        {
            Message = message,
            SenderId = User.GetUserId(),
            SentAt = DateTime.UtcNow
        });
        _dbContext.SellerServiceOrders.Update(order);
        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }
    
    
    [HttpPut("{orderId:int}/Terminate")]
    public async Task<IActionResult> TerminateOrder(int orderId)
    {
        var order = await _dbContext.SellerServiceOrders.Include(x=>x.Seller).ThenInclude(x=>x.User).Include(x=>x.Buyer)
            .FirstOrDefaultAsync(x=>x.Id==orderId);

        if (order == null)
            return NotFound("Order not found.");
        
        order.Status = EnumOrderStatus.Cancelled;
        _dbContext.SellerServiceOrders.Update(order);
        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }
}