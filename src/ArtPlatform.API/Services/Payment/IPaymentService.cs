using ArtPlatform.Database.Entities;

namespace ArtPlatform.API.Services.Payment;

public interface IPaymentService
{
    string CreateSellerAccount();
    string CreateSellerAccountOnboardingUrl(string accountId);
}