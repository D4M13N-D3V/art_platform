 using ArtPlatform.Database.Entities;
 using comissions.app.database.Entities;

 namespace comissions.app.api.Models.Order;

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
            Price = sellerProfile.Price,
            PaymentUrl = sellerProfile.PaymentUrl
        };
    }
}