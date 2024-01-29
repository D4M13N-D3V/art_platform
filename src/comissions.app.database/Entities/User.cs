using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace comissions.app.database.Entities;

[PrimaryKey(nameof(Id))]
public record User
{
    public string Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Biography { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int? UserSellerProfileId { get; set; }
    
    public bool Banned { get; set; } = false;
    public DateTime? BannedDate { get; set; }
    public DateTime? UnbanDate { get; set; }
    public string? BannedReason { get; set; }
    public string? BanAdminId { get; set; }
    
    public bool Suspended { get; set; } = false;
    public DateTime? SuspendedDate { get; set; }
    public DateTime? UnsuspendDate { get; set; }
    public string? SuspendedReason { get; set; }
    public string? SuspendAdminId { get; set; }
    
    [JsonIgnore] public virtual UserSellerProfile? UserSellerProfile { get; set; }
    [JsonIgnore] public virtual ICollection<SellerServiceOrder> Orders { get; set; }
}