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
public class OrderController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorage _storage;
 
    public OrderController(ApplicationDbContext dbContext, IStorage storage)
    {
        _storage = storage;
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("Orders")]
    [Authorize("read:orders")]
    public async Task<IActionResult> GetOrders(int offset = 0, int pageSize = 10, EnumOrderStatus? status = null)
    {
        var userId = User.GetUserId();
        var orders = await _dbContext.SellerServiceOrders
            .Where(x => x.BuyerId == userId && status==null ? true : status==x.Status)
            .Skip(offset).Take(pageSize).ToListAsync();
        var result = orders.Select(x => x.ToModel()).ToList();
        return Ok(result);
    }
    
    [HttpPost]
    [Route("Sellers/{sellerId:int}/Services/{serviceId:int}")]
    [Authorize("write:orders")]
    public async Task<IActionResult> CreateOrder(int sellerId, int serviceId)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound("Seller not found.");
        var service = await _dbContext.SellerServices
            .Include(x=>x.Reviews)
            .FirstOrDefaultAsync(x=>x.Id==serviceId);
        if(service==null)
            return NotFound("Service not found.");
        
        if(_dbContext.SellerServiceOrders.Where(x=>x.BuyerId==userId && x.Status!=EnumOrderStatus.Completed && x.Status!=EnumOrderStatus.Cancelled).Count()>=3)
            return BadRequest("You already have an order in progress. There is a limit of three at a time.");
        
        var order = new SellerServiceOrder()
        {
            BuyerId = userId,
            SellerId = seller.Id,
            SellerServiceId = serviceId,
            Status = EnumOrderStatus.PendingAcceptance,
            CreatedDate = DateTime.UtcNow,
            Price = service.Price,
            SellerService = service,
            Buyer = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId),
        };
        order = _dbContext.SellerServiceOrders.Add(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:orders")]
    [Route("Orders/{orderId:int}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.BuyerId!=userId)
            return BadRequest("You are not the buyer of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("Order is not in a cancellable state.");
        order.Status = EnumOrderStatus.Cancelled;
        order.EndDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:orders")]
    [Route("Orders/{orderId:int}/AcceptPrice")]
    public async Task<IActionResult> AcceptPrice(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .Include(x=>x.Seller)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("Order is already complete.");
        if(order.Status<EnumOrderStatus.DiscussingRequirements)
            return BadRequest("Order has not been started yet.");
        order.Status = EnumOrderStatus.InProgress;
        order.TermsAcceptedDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:orders")]
    [Route("Orders/{orderId:int}/Accept")]
    public async Task<IActionResult> Accept(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("Order is already complete.");
        if(order.Status<EnumOrderStatus.InProgress)
            return BadRequest("Order has not been started yet.");
        if(order.Status<EnumOrderStatus.PendingReview)
            return BadRequest("Order is in progress and not pending review.");
        order.Status = EnumOrderStatus.Completed;
        order.TermsAcceptedDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:orders")]
    [Route("Orders/{orderId:int}/Deny")]
    public async Task<IActionResult> Deny(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("Order is already complete.");
        if(order.Status<EnumOrderStatus.InProgress)
            return BadRequest("Order has not been started yet.");
        if(order.Status<EnumOrderStatus.PendingReview)
            return BadRequest("Order is in progress and not pending review.");
        order.Status = EnumOrderStatus.InProgress;
        order.TermsAcceptedDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize("write:orders")]
    [Route("Orders/{orderId:int}/Review")]
    public async Task<IActionResult> Review(int orderId, [FromBody] SellerServiceOrderReviewModel model)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Reviews)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("Order not found.");
        if(order.BuyerId!=userId)
            return BadRequest("You are not the buyer of this order.");
        if(order.Status!=EnumOrderStatus.Completed)
            return BadRequest("Order is not complete.");
        if(order.Reviews.Any(x=>x.SellerServiceOrderId==orderId))
            return BadRequest("Order has already been reviewed.");
        var review = new SellerServiceOrderReview()
        {
            SellerServiceOrderId = orderId,
            SellerServiceId = order.SellerServiceId,
            Rating = model.Rating,
            Review = model.Review,
            ReviewDate = DateTime.UtcNow,
            ReviewerId = userId,
            Reviewer = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id==userId),
        };
        await _dbContext.SellerServiceOrderReviews.AddAsync(review);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet]
    [Authorize("read:orders")]
    [Route("Orders/{orderId:int}/Messages")]
    public async Task<IActionResult> GetMessages(int orderId, int offset = 0, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
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
    [Route("Orders/{orderId:int}/Message")]
    public async Task<IActionResult> Message(int orderId, [FromBody] SellerServiceOrderMessageModel model)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Messages)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
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
    [Route("Orders/{orderId:int}/Message/{messageId:int}/Attachment")]
    public async Task<IActionResult> MessageAttachment(int orderId, int messageId,IFormFile file)
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
        
        var message = _dbContext.SellerServiceOrderMessages.First(x=>x.Id==messageId && x.SellerServiceOrderId==orderId);
        if(message==null)
            return BadRequest("Message does not exist or does not belong to this order.");
        
        var url = await _storage.UploadImageAsync(file, Guid.NewGuid().ToString());
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
    [Route("Orders/{orderId:int}/Message/{messageId:int}/Attachment")]
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
        
        var content = await _storage.DownloadImageAsync(message.Attachments.First().FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }
}
