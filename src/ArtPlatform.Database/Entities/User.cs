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
    
    [JsonIgnore] public virtual UserSellerProfile? UserSellerProfile { get; set; }
    [JsonIgnore] public virtual ICollection<SellerServiceOrder> Orders { get; set; }
}