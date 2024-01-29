namespace comissions.app.database.Entities;

public class SellerService
{
    public int Id { get; set; }
    public int SellerProfileId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Price { get; set; }
    public bool Archived { get; set; } = false;

    public virtual ICollection<SellerProfilePortfolioPiece> PortfolioPieces { get; set; } = new List<SellerProfilePortfolioPiece>();
    public virtual ICollection<SellerServiceOrderReview> Reviews { get; set; } = new List<SellerServiceOrderReview>();
    public virtual UserSellerProfile SellerProfile { get; set; } = null!;
}