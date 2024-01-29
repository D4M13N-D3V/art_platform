namespace comissions.app.api.Models.User;

public class UserInfoModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Biography { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}