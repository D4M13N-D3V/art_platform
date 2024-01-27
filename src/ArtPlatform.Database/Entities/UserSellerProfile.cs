using System.ComponentModel.DataAnnotations.Schema;

namespace ArtPlatform.Database.Entities;

public record UserSellerProfile
{
    public int Id { get; set; }
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public string Biography { get; set; }
    public List<string> SocialMediaLinks { get; set; } = new();
    public bool AgeRestricted { get; set; }
    public string? StripeAccountId { get; set; } 
    public virtual User User { get; set; } = null!;

    public virtual ICollection<SellerService> SellerServices { get; set; } = new List<SellerService>();
    public virtual ICollection<SellerProfilePortfolioPiece> PortfolioPieces { get; set; } = new List<SellerProfilePortfolioPiece>();
}