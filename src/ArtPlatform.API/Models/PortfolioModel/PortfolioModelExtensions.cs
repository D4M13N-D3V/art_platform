namespace ArtPlatform.API.Models.PortfolioModel;

public static class PortfolioModelExtensions
{
    public static PortfolioModel ToModel(this Database.Entities.SellerProfilePortfolioPiece sellerProfileRequest)
    {
        return new PortfolioModel()
        {
            Id = sellerProfileRequest.Id,
            SellerServiceId = sellerProfileRequest.SellerServiceId
        };
    }
}