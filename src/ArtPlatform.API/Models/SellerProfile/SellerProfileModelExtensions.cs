using ArtPlatform.API.Models.Discovery;
using ArtPlatform.Database.Entities;

namespace ArtPlatform.API.Models.SellerProfile;

public static class SellerProfileModelExtensions
{
    public static SellerProfileModel ToModel(this UserSellerProfile sellerProfile)
    {
        return new SellerProfileModel()
        {
            SocialMediaLinks = sellerProfile.SocialMediaLinks,
            Biography = sellerProfile.Biography,
            PrepaymentRequired = sellerProfile.PrepaymentRequired
        };
    }
    public static DiscoverySellerModel ToDiscoveryModel(this UserSellerProfile sellerProfile)
    {
        return new DiscoverySellerModel()
        {
            Id = sellerProfile.Id,
            SocialMediaLinks = sellerProfile.SocialMediaLinks,
            Biography = sellerProfile.Biography,
            PrepaymentRequired = sellerProfile.PrepaymentRequired
        };
    }
    public static UserSellerProfile ToModel(this SellerProfileModel sellerProfile, UserSellerProfile existingSellerProfile)
    {
        existingSellerProfile.SocialMediaLinks = sellerProfile.SocialMediaLinks;
        existingSellerProfile.Biography = sellerProfile.Biography;
        existingSellerProfile.PrepaymentRequired = sellerProfile.PrepaymentRequired;
        return existingSellerProfile;
    }
}