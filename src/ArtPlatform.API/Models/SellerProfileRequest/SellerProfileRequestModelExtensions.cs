namespace ArtPlatform.API.Models.SellerProfileRequest;

public static class SellerProfileRequestModelExtensions
{
    public static SellerProfileRequestModel ToModel(this Database.Entities.SellerProfileRequest sellerProfileRequest)
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