using comissions.app.database.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace comissions.app.database;

public class ApplicationDbContext:DbContext
{
    private readonly ApplicationDatabaseConfigurationModel _configuration;
    
    // public ApplicationDbContext(ApplicationDatabaseConfigurationModel configuration, DbContextOptions<ApplicationDbContext> options):base(options)
    // {
    //     _configuration = configuration;
    // }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
        {
            Host = _configuration?.Host ?? "localhost",
            Port = _configuration?.Port ?? 5432,
            Database = _configuration?.Database ?? "artplatform",
            Username = _configuration?.Username ?? "sa",
            Password = _configuration?.Password ?? "P@ssw0rd"
        };
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
    
    #region DB Sets

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserSellerProfile> UserSellerProfiles { get; set; }= null!;
    public DbSet<SellerProfileRequest> SellerProfileRequests { get; set; }= null!;
    public DbSet<SellerProfilePortfolioPiece> SellerProfilePortfolioPieces { get; set; }= null!;
    public DbSet<SellerService> SellerServices { get; set; }= null!;
    public DbSet<SellerServiceOrder> SellerServiceOrders { get; set; }= null!;
    public DbSet<SellerServiceOrderMessage> SellerServiceOrderMessages { get; set; }= null!;
    public DbSet<SellerServiceOrderMessageAttachment> SellerServiceOrderMessageAttachments { get; set; }= null!;
    public DbSet<SellerServiceOrderReview> SellerServiceOrderReviews { get; set; }= null!;
    #endregion
}