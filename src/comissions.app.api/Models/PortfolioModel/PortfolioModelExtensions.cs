using ArtPlatform.Database.Entities;
using comissions.app.database.Entities;

namespace comissions.app.api.Models.PortfolioModel;

public static class PortfolioModelExtensions
{
    public static PortfolioModel ToModel(this SellerProfilePortfolioPiece sellerProfileRequest)
    {
        return new PortfolioModel()
        {
            Id = sellerProfileRequest.Id,
            SellerServiceId = sellerProfileRequest.SellerServiceId
        };
    }
}