namespace comissions.app.database.Entities;

public class SellerServiceOrderMessageAttachment
{
    public int Id { get; set; }
    public int SellerServiceOrderMessageId { get; set; }
    public string FileReference { get; set; } = null!;
    
    public virtual SellerServiceOrderMessage SellerServiceOrderMessage { get; set; } = null!;
}