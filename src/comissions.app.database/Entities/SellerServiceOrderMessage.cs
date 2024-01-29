namespace comissions.app.database.Entities;

public class SellerServiceOrderMessage
{
    public int Id { get; set; }
    public int SellerServiceOrderId { get; set; }
    public string SenderId { get; set; }
    public string Message { get; set; } = null!;
    public DateTime SentAt { get; set; }
    
    public virtual SellerServiceOrder SellerServiceOrder { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
    public virtual ICollection<SellerServiceOrderMessageAttachment> Attachments { get; set; } = null!;
}