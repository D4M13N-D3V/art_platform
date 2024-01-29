using ArtPlatform.Database.Entities;

namespace comissions.app.api.Services.Payment;

public interface IPaymentService
{
    public string CreateCustomer();
    string CreateSellerAccount();
    string CreateSellerAccountOnboardingUrl(string accountId); 
    bool SellerAccountIsOnboarded(string accountId);
    string ChargeForService(int orderSellerServiceId, string? sellerStripeAccountId, double orderPrice);
}