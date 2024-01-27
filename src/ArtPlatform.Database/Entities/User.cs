using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Database.Entities;

[PrimaryKey(nameof(Id))]
public record User
{
    public string Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Biography { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int? UserSellerProfileId { get; set; }
    
    #region Billing Information
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string AddressCountry { get; set; } = null!;
    public string AddressCity { get; set; } = null!;
    public string AddressStreet { get; set; } = null!;
    public string AddressHouseNumber { get; set; } = null!;
    public string AddressPostalCode { get; set; } = null!;
    public string AddressRegion { get; set; } = null!;
    #endregion
    
    [JsonIgnore] public virtual UserSellerProfile? UserSellerProfile { get; set; }
    [JsonIgnore] public virtual ICollection<SellerServiceOrder> Orders { get; set; }
}