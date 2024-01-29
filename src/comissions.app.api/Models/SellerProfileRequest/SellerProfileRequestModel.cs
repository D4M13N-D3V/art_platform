namespace comissions.app.api.Models.SellerProfileRequest;

public class SellerProfileRequestModel
{
    public int Id { get; set; }
    public DateTime RequestDate { get; set; }
    public string UserId { get; set; }
    public bool Accepted { get; set; }
    
    public virtual database.Entities.User User { get; set; } = null!;
}