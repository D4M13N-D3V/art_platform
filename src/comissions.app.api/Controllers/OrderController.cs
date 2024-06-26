using comissions.app.api.Extensions;
using ArtPlatform.Database;
using ArtPlatform.Database.Entities;
using comissions.app.api.Models.Order;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using comissions.app.database;
using comissions.app.database.Entities;
using comissions.app.database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace comissions.app.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IPaymentService _paymentService;
    private readonly string _webHookSecret;
    
    public OrderController(ApplicationDbContext dbContext, IConfiguration configuration, IStorageService storageService, IPaymentService paymentService)
    {
        _paymentService = paymentService;
        _storageService = storageService;
        _dbContext = dbContext;
        _webHookSecret = configuration.GetValue<string>("Stripe:WebHookSecret");
    }

    [HttpPost("PaymentWebhook")]
    public async Task<IActionResult> ProcessWebhookEvent()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        // If you are testing your webhook locally with the Stripe CLI you
        // can find the endpoint's secret by running `stripe listen`
        // Otherwise, find your endpoint's secret in your webhook settings
        // in the Developer Dashboard
        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webHookSecret);

        if (stripeEvent.Type == Events.CheckoutSessionExpired)
        {
            var session = stripeEvent.Data.Object as Session;
            var connectedAccountId = stripeEvent.Account;
            var orderId = session.LineItems.First().Price.Product.Name;
            var order = await _dbContext.SellerServiceOrders
                .Include(x=>x.Seller)
                .Include(x=>x.Buyer)
                .Include(x=>x.SellerService)
                .FirstOrDefaultAsync(x=>x.Id==int.Parse(orderId));
            if (order != null && order.Status == EnumOrderStatus.WaitingForPayment)
            {
                order.PaymentUrl = null;
            }
        }
        else if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session;
            var connectedAccountId = stripeEvent.Account;
            var orderId = session.Metadata["/OrderId"];
            var order = await _dbContext.SellerServiceOrders
                .Include(x=>x.Seller)
                .Include(x=>x.SellerService)
                .FirstOrDefaultAsync(x=>x.Id==int.Parse(orderId));
            if (order != null && order.Seller.StripeAccountId==connectedAccountId && order.Status == EnumOrderStatus.WaitingForPayment)
            {
                if (order.Seller.PrepaymentRequired)
                    order.Status = EnumOrderStatus.InProgress;
                else
                    order.Status = EnumOrderStatus.Completed;
            }
        }
        return Ok();
    }
    
    [HttpGet]
    [Route("/Orders")]
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
    [HttpGet]
    [Route("/Orders/{orderId:int}")]
    [Authorize("read:orders")]
    public async Task<IActionResult> GetOrder(int orderId,int offset = 0, int pageSize = 10, EnumOrderStatus? status = null)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .FirstAsync(x => x.Id==orderId && x.BuyerId == userId && status == null ? true : status == x.Status);
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPost]
    [Route("/Sellers/{sellerId:int}/Services/{serviceId:int}")]
    [Authorize("write:orders")]
    public async Task<IActionResult> CreateOrder(int sellerId, int serviceId)
    {
        var userId = User.GetUserId();
        var seller = await _dbContext.UserSellerProfiles
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x=>x.Id==sellerId);
        if(seller==null)
            return NotFound("Seller not found.");
        if(seller.Suspended)
            return NotFound("Seller is suspended.");
        
        var service = await _dbContext.SellerServices
            .Include(x=>x.Reviews)
            .FirstOrDefaultAsync(x=>x.Id==serviceId);
        if(service==null)
            return NotFound("Service not found.");
        
        if(service.Archived)
            return BadRequest("Service is archived.");
        
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
    [Route("/api/Orders/{orderId:int}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.BuyerId!=userId)
            return BadRequest("You are not the buyer of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("/Order is not in a cancellable state.");
        order.Status = EnumOrderStatus.Cancelled;
        order.EndDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize("write:orders")]
    [Route("/api/Orders/{orderId:int}/AcceptPrice")]
    public async Task<IActionResult> AcceptPrice(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .Include(x=>x.Buyer)
            .Include(x=>x.Seller)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("/Order is already complete.");
        if(order.Status<EnumOrderStatus.DiscussingRequirements)
            return BadRequest("/Order has not been started yet.");

        if(string.IsNullOrEmpty(order.PaymentUrl)==false)
            return BadRequest("/Order has price already been agreed on.");
        
        if(order.Status==EnumOrderStatus.WaitingForPayment)
            return BadRequest("/Order is waiting for payment.");
        
        order.TermsAcceptedDate = DateTime.UtcNow;

        if (order.Seller.PrepaymentRequired)
        {
            order.Status = EnumOrderStatus.WaitingForPayment;
            var url = _paymentService.ChargeForService(order.Id, order.Seller.StripeAccountId, order.Price);
            order.PaymentUrl = url;
        }
        else
        {
            order.Status = EnumOrderStatus.InProgress;
        }
        
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    [HttpPut]
    [Authorize("write:orders")]
    [Route("/api/Orders/{orderId:int}/Payment")]
    public async Task<IActionResult> Payment(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.SellerService)
            .Include(x=>x.Buyer)
            .Include(x=>x.Seller)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("/Order is already complete.");
        if(order.Status!=EnumOrderStatus.WaitingForPayment)
            return BadRequest("/Order does not need to be paid for.");
        if (order.PaymentUrl != null)
            return Ok(order.PaymentUrl);
        var url = _paymentService.ChargeForService(order.Id, order.Seller.StripeAccountId, order.Price);
        order.PaymentUrl = url;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        return Ok(order.PaymentUrl);
    }
    
    [HttpPut]
    [Authorize("write:orders")]
    [Route("/api/Orders/{orderId:int}/Accept")]
    public async Task<IActionResult> Accept(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("/Order is already complete.");
        if(order.Status<EnumOrderStatus.InProgress)
            return BadRequest("/Order has not been started yet.");
        if(order.Status<EnumOrderStatus.PendingReview)
            return BadRequest("/Order is in progress and not pending review.");
        if(order.Status==EnumOrderStatus.WaitingForPayment)
            return BadRequest("/Order is waiting for payment.");
        
        if(order.Seller.PrepaymentRequired)
            order.Status = EnumOrderStatus.Completed;
        else
        {
            order.Status = EnumOrderStatus.WaitingForPayment;
            var url = _paymentService.ChargeForService(order.Id, order.Seller.StripeAccountId, order.Price);
            order.PaymentUrl = url;
        }
        
        order.TermsAcceptedDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize("write:orders")]
    [Route("/api/Orders/{orderId:int}/Deny")]
    public async Task<IActionResult> Deny(int orderId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.Seller.UserId!=userId)
            return BadRequest("You are not the seller of this order.");
        if(order.Status==EnumOrderStatus.Completed)
            return BadRequest("/Order is already complete.");
        if(order.Status<EnumOrderStatus.InProgress)
            return BadRequest("/Order has not been started yet.");
        if(order.Status<EnumOrderStatus.PendingReview)
            return BadRequest("/Order is in progress and not pending review.");
        order.Status = EnumOrderStatus.InProgress;
        order.TermsAcceptedDate = DateTime.UtcNow;
        order = _dbContext.SellerServiceOrders.Update(order).Entity;
        await _dbContext.SaveChangesAsync();
        var result = order.ToModel();
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize("write:orders")]
    [Route("/api/Orders/{orderId:int}/Review")]
    public async Task<IActionResult> Review(int orderId, [FromBody] SellerServiceOrderReviewModel model)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Reviews)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.BuyerId!=userId)
            return BadRequest("You are not the buyer of this order.");
        if(order.Status!=EnumOrderStatus.Completed)
            return BadRequest("/Order is not complete.");
        if(order.Reviews.Any(x=>x.SellerServiceOrderId==orderId))
            return BadRequest("/Order has already been reviewed.");
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
    [Route("/api/Orders/{orderId:int}/Messages")]
    public async Task<IActionResult> GetMessages(int orderId, int offset = 0, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Seller)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
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
    [Route("/api/Orders/{orderId:int}/Message")]
    public async Task<IActionResult> Message(int orderId, [FromBody] SellerServiceOrderMessageModel model)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Messages)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId && x.BuyerId==userId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.Status==EnumOrderStatus.Completed || order.Status==EnumOrderStatus.Cancelled)
            return BadRequest("/Order is already complete.");
        if(order.BuyerId!=userId && order.Seller.UserId!=userId)
            return BadRequest("You are not the buyer or seller of this order.");
        if(order.Status<EnumOrderStatus.Waitlist)
            return BadRequest("/Order is not accepted.");
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
    [Route("/api/Orders/{orderId:int}/Message/{messageId:int}/Attachment")]
    public async Task<IActionResult> MessageAttachment(int orderId, int messageId,IFormFile file)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Messages)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.BuyerId!=userId && order.Seller.UserId!=userId)
            return BadRequest("You are not the buyer or seller of this order.");
        if(order.Status==EnumOrderStatus.Completed || order.Status==EnumOrderStatus.Cancelled)
            return BadRequest("/Order is already complete.");
        if(order.Status<EnumOrderStatus.Waitlist)
            return BadRequest("/Order is not accepted.");
        
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
    [Route("/api/Orders/{orderId:int}/Message/{messageId:int}/Attachment")]
    public async Task<IActionResult> MessageAttachments(int orderId, int messageId)
    {
        var userId = User.GetUserId();
        var order = await _dbContext.SellerServiceOrders
            .Include(x=>x.Messages)
            .Include(x=>x.Seller)
            .Include(x=>x.SellerService)
            .FirstOrDefaultAsync(x=>x.Id==orderId);
        if(order==null)
            return NotFound("/Order not found.");
        if(order.BuyerId!=userId && order.Seller.UserId!=userId)
            return BadRequest("You are not the buyer or seller of this order.");
        if(order.Status==EnumOrderStatus.Completed || order.Status==EnumOrderStatus.Cancelled)
            return BadRequest("/Order is already complete.");
        if(order.Status<EnumOrderStatus.Waitlist)
            return BadRequest("/Order is not accepted.");
        
        var message = _dbContext.SellerServiceOrderMessages.Include(x=>x.Attachments)
            .First(x=>x.Id==messageId && x.SellerServiceOrderId==orderId);
        if(message==null)
            return BadRequest("Message does not exist or does not belong to this order.");
        
        var content = await _storageService.DownloadImageAsync(message.Attachments.First().FileReference);
        return new FileStreamResult(content, "application/octet-stream");
    }
}

