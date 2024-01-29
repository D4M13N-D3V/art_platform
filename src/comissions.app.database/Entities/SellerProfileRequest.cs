namespace comissions.app.database.Entities;

public class SellerProfileRequest
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public bool Accepted { get; set; }
    
    public virtual User User { get; set; } = null!;
}