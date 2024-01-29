namespace comissions.app.database.Entities;

public class SellerProfilePortfolioPiece
{
    public int Id { get; set; }
    public int SellerProfileId { get; set; }
    public string FileReference { get; set; }
    public int? SellerServiceId { get; set; }
    public virtual SellerService SellerService { get; set; } = null!;
    public virtual UserSellerProfile SellerProfile { get; set; } = null!;
}