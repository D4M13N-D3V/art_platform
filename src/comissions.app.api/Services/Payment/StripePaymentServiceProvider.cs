using ArtPlatform.Database.Entities;
using Stripe;

namespace comissions.app.api.Services.Payment;

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

    public string CreateCustomer()
    {
        var options = new CustomerCreateOptions
        {
            Name = "Jenny Rosen",
            Email = "jennyrosen@example.com",
        };
        var service = new CustomerService();
        var customer = service.Create(options);
        return customer.Id;
    }
    
    // public string ChargeCustomer(string customerId, int amount)
    // {
    //     var options = new PaymentIntentCreateOptions
    //     {
    //         Amount = amount,
    //         Currency = "usd",
    //         AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
    //         {
    //             Enabled = true,
    //         },
    //     };
    //     var requestOptions = new RequestOptions
    //     {
    //         StripeAccount = "{{CONNECTED_ACCOUNT_ID}}",
    //     };
    //     var service = new PaymentIntentService();
    //     var intent = service.Create(options, requestOptions);
    //     throw new NotImplementedException();
    // }
    public string CreateSellerAccount()
    {
        var accountCreateOptions = new AccountCreateOptions { Type = "express",
            Capabilities
                = new AccountCapabilitiesOptions
                {
                    CardPayments
                        = new AccountCapabilitiesCardPaymentsOptions { Requested
                            = true },
                    Transfers
                        = new AccountCapabilitiesTransfersOptions { Requested
                            = true },
                } };
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

    public string ChargeForService(int orderSellerServiceOrderId, string? sellerStripeAccountId,
        double orderPrice)
    {
        var feeAmount = (long)Math.Round((orderPrice*0.05) * 100);
        var options = new Stripe.Checkout.SessionCreateOptions
        {
            LineItems = new List<Stripe.Checkout.SessionLineItemOptions> {
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)Math.Round(orderPrice * 100),
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Comission Service",
                            },
                        },
                        Quantity = 1,
                    },
                },
            PaymentIntentData = new Stripe.Checkout.SessionPaymentIntentDataOptions
                {
                    ApplicationFeeAmount = feeAmount,
                },
            Mode = "payment",
            SuccessUrl = "https://example.com/success",
            CancelUrl = "https://example.com/failure",
            Metadata = new Dictionary<string, string>()
            {
                ["orderId"] = orderSellerServiceOrderId.ToString()
            }
        };
        var requestOptions = new RequestOptions
        {
            StripeAccount = sellerStripeAccountId
        };
        var service = new Stripe.Checkout.SessionService();
        var session = service.Create(options, requestOptions);
        return session.Url;
    }
}