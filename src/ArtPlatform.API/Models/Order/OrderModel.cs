using ArtPlatform.Database.Enums;

namespace ArtPlatform.API.Models.Order;

public class OrderModel
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public int SellerServiceId { get; set; }
    public int SellerId { get; set; }
    public EnumOrderStatus Status { get; set; }

    public string StatusLabel => Status.ToString();
    public double Price { get; set; }
    public string? PaymentUrl { get; set; }
}