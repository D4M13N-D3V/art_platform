namespace comissions.app.api.Models.SellerProfileRequest;

public static class SellerProfileRequestModelExtensions
{
    public static SellerProfileRequestModel ToModel(this database.Entities.SellerProfileRequest sellerProfileRequest)
    {
        return new SellerProfileRequestModel()
        {
            Id = sellerProfileRequest.Id,
            UserId = sellerProfileRequest.UserId,
            RequestDate = sellerProfileRequest.RequestDate,
            Accepted = sellerProfileRequest.Accepted
        };
    }
}