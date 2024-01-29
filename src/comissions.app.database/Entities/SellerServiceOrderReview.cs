namespace comissions.app.database.Entities;

public class SellerServiceOrderReview
{
    public int Id { get; set; }
    public string ReviewerId { get; set; }
    public int SellerServiceOrderId { get; set; }
    public int SellerServiceId { get; set; }
    public DateTime ReviewDate { get; set; }
    public string? Review { get; set; }
    public int Rating { get; set; }
    
    public virtual User Reviewer { get; set; } = null!;
    public virtual SellerServiceOrder SellerServiceOrder { get; set; } = null!;
    public virtual SellerService SellerService { get; set; } = null!;
}