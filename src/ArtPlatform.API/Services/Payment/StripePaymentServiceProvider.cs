using ArtPlatform.Database.Entities;
using Stripe;

namespace ArtPlatform.API.Services.Payment;

public class StripePaymentServiceProvider:IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    
    
    public StripePaymentServiceProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiKey = _configuration.GetValue<string>("Stripe:ApiKey");
        StripeConfiguration.ApiKey = _apiKey;
    }
    
    public string CreateSellerAccount()
    {
        var accountCreateOptions = new AccountCreateOptions { Type = "express" };
        var accountService = new AccountService();
        var account = accountService.Create(accountCreateOptions);
        return account.Id;
    }
    
    public string CreateSellerAccountOnboardingUrl(string accountId)
    {
        var options = new AccountLinkCreateOptions
        {
            Account = accountId,
            RefreshUrl = "https://example.com/reauth",
            ReturnUrl = "https://example.com/return",
            Type = "account_onboarding",
        };
        var service = new AccountLinkService();
        var url = service.Create(options);
        return url.Url;
    }
    
    public bool SellerAccountIsOnboarded(string accountId)
    {
        var service = new AccountService();
        var account = service.Get(accountId);
        return account.Requirements.CurrentlyDue.Count == 0 && account.ChargesEnabled==true && account.DetailsSubmitted==true;
    }
}