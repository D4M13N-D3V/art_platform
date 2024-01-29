namespace comissions.app.api.Models.SellerService;

public class SellerServiceModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Price { get; set; }
    public double AverageRating { get; set; }
    public int NumberOfRatings { get; set; }
}