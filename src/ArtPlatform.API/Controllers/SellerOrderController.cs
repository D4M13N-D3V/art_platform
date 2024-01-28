using ArtPlatform.API.Extensions;
using ArtPlatform.API.Models.Order;
using ArtPlatform.API.Services.Storage;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using ArtPlatform.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerOrderController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    
    public SellerOrderController(IStorageService storageService, ApplicationDbContext dbContext)
    {
        _storageService = storageService;
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("/SellerOrders")]
    [Authorize("read:seller-orders")]
    public async Task<IActionResult> GetOrders(int offset = 0, int pageSize = 10, EnumOrderStatus? status = null)
    {
        var userId = User.GetUserId();
        var orders = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Where(x => x.Seller.UserId == userId && status==null ? true : status==x.Status)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = orders.Select(x => x.ToModel()).ToList();
        return Ok(result);
    }
    [HttpGet]
    [Route("/SellerOrders/{orderId:int}")]
    [Authorize("read:seller-orders")]
    public async Task<IActionResult> GetOrder(int orderId, int offset = 0, int pageSize = 10, EnumOrderStatus? status = null)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x => x.Seller)
            .FirstAsync(x => x.Id==orderId && x.Seller.UserId == userId && status == null ? true : status == x.Status);
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/Cancel")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Status==EnumOrderStatus.Completed || order.Status== EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.BuyerId!=userId)
            return BadRequest("You are not the buyer of this order.");
        if(order.Status!=EnumOrderStatus.Completed && order.Status!= EnumOrderStatus.Cancelled)
            return BadRequest("Order is not in a cancellable state.");
        order.Status = EnumOrderStatus.Cancelled;
        order.EndDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/Accept")]
    public async Task<IActionResult> AcceptOrder(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Status==EnumOrderStatus.Completed || order.Status== EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.BuyerId!=userId)
            return BadRequest("You are not the buyer of this order.");
        if(order.Status!=EnumOrderStatus.PendingAcceptance)
            return BadRequest("Order has already been accepted.");
        order.Status = EnumOrderStatus.Waitlist;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/Start")]
    public async Task<IActionResult> StartOrder(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Status==EnumOrderStatus.Completed || order.Status== EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.BuyerId!=userId)
            return BadRequest("You are not the buyer of this order.");
        if(order.Status!=EnumOrderStatus.Waitlist)
            return BadRequest("Order has already been started.");
        order.Status = EnumOrderStatus.DiscussingRequirements;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/AdjustPrice")]
    public async Task<IActionResult> AdjustPrice(int orderId,[FromQuery]double price)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed || order.Status== EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.Status>EnumOrderStatus.DiscussingRequirements)
            return BadRequest("Order requirements and price have already been confirmed.");
        if(order.Status<EnumOrderStatus.DiscussingRequirements)
            return BadRequest("Order has not been started.");
        order.Price = price;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:seller-orders")]
    [Route("/api/SellerOrders/{orderId:int}/CompleteRevision")]
    public async Task<IActionResult> CompleteRevision(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed || order.Status== EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.Status<EnumOrderStatus.InProgress)
            return BadRequest("Order has not been started.");
        if(order.Status>EnumOrderStatus.InProgress)
            return BadRequest("Order is pending review already.");
        order.Status = EnumOrderStatus.PendingReview;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize("read:orders")]
    [Route("/api/SellerOrders/{orderId:int}/Messages")]
    public async Task<IActionResult> GetMessages(int orderId, int offset = 0, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.BuyerId!=userId && order.Seller.UserId!=userId)
            return BadRequest("You are not the buyer or seller of this order.");
        var messages = _dbContext.SellerServiceOrderMessages
            .Include(x=>x.Sender)
            .Include(x=>x.Attachments)
            .OrderBy(x=>x.SentAt)
            .Where(x=>x.SellerServiceOrderId==orderId)
            .Skip(offset).Take(pageSize).ToList();
        var result = messages.Select(x=>x.ToModel()).ToList();
        return Ok(result);
    }
    [HttpPost]
    [Authorize("write:orders")]
    [Route("/api/SellerOrders/{orderId:int}/Message")]
    public async Task<IActionResult> Message(int orderId, [FromBody] SellerServiceOrderMessageModel model)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Messages)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Status==EnumOrderStatus.Completed || order.Status==EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.BuyerId!=userId && order.Seller.UserId!=userId)
            return BadRequest("You are not the buyer or seller of this order.");
        if(order.Status<EnumOrderStatus.Waitlist)
            return BadRequest("Order is not accepted.");
        var message = new SellerServiceOrderMessage()
        {
            SellerServiceOrderId = orderId,
            Message = model.Message,
            SentAt = DateTime.UtcNow,
            SenderId = userId,
            Sender = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId),
        };
        var dbMessage = _dbContext.SellerServiceOrderMessages.Add(message).Entity;
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPost]
    [Authorize("write:orders")]
    [Route("/api/SellerOrders/{orderId:int}/Message/{messageId:int}/Attachment")]
    public async Task<IActionResult> MessageAttachment(int orderId, int messageId,IFormFile file)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Messages)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.Seller.UserId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.BuyerId!=userId && order.Seller.UserId!=userId)
            return BadRequest("You are not the buyer or seller of this order.");
        if(order.Status==EnumOrderStatus.Completed || order.Status==EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.Status<EnumOrderStatus.Waitlist)
            return BadRequest("Order is not accepted.");
        
        var message = _dbContext.SellerServiceOrderMessages.First(x=>x.Id==messageId && x.SellerServiceOrderId==orderId);
        if(message==null)
            return BadRequest("Message does not exist or does not belong to this order.");
        
        var url = await _storageService.UploadImageAsync(file, Guid.NewGuid().ToString());
        var attachment = new SellerServiceOrderMessageAttachment()
        {
            SellerServiceOrderMessageId = message.Id,
            FileReference = url
        };
        _dbContext.SellerServiceOrderMessageAttachments.Add(attachment);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    [HttpGet]
    [Authorize("read:orders")]
    [Route("/api/SellerOrders/{orderId:int}/Message/{messageId:int}/Attachment")]
    public async Task<IActionResult> MessageAttachments(int orderId, int messageId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Messages)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.BuyerId!=userId && order.Seller.UserId!=userId)
            return BadRequest("You are not the buyer or seller of this order.");
        if(order.Status==EnumOrderStatus.Completed || order.Status==EnumOrderStatus.Cancelled)
            return BadRequest("Order is already complete.");
        if(order.Status<EnumOrderStatus.Waitlist)
            return BadRequest("Order is not accepted.");
        
        var message = _dbContext.SellerServiceOrderMessages.Include(x=>x.Attachments)
            .First(x=>x.Id==messageId && x.SellerServiceOrderId==orderId);
        if(message==null)
            return BadRequest("Message does not exist or does not belong to this order.");
        var attachment = message.Attachments.FirstOrDefault();
        if(attachment==null)
            return BadRequest("Message does not have an attachment.");
        var content = await _storageService.DownloadImageAsync(message.Attachments.First().FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }
}