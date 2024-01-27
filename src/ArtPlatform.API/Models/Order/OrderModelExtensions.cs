using ArtPlatform.Database.Entities;

namespace ArtPlatform.API.Models.Order;

public static class OrderModelExtensions
{
    
    public static OrderModel ToModel(this SellerServiceOrder sellerProfile)
    {
        return new OrderModel()
        {
            Id = sellerProfile.Id,
            BuyerId = sellerProfile.BuyerId,
            SellerServiceId = sellerProfile.SellerServiceId,
            SellerId = sellerProfile.SellerId,
            Status = sellerProfile.Status,
            Price = sellerProfile.Price
        };
    }
}