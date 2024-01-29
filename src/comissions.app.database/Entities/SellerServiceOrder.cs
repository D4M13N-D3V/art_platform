using comissions.app.database.Enums;

namespace comissions.app.database.Entities;

public class SellerServiceOrder
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public int SellerServiceId { get; set; }
    public int SellerId { get; set; }
    public EnumOrderStatus Status { get; set; }
    public double Price { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? TermsAcceptedDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public virtual User Buyer { get; set; } = null!;
    public virtual SellerService SellerService { get; set; } = null!;
    public virtual UserSellerProfile Seller { get; set; } = null!;

    public virtual ICollection<SellerServiceOrderReview> Reviews { get; set; } = new List<SellerServiceOrderReview>();
    public virtual ICollection<SellerServiceOrderMessage> Messages { get; set; } = new List<SellerServiceOrderMessage>();
    public string? PaymentUrl { get; set; }
}