namespace ArtPlatform.API.Models.User;

public class UserInfoUpdateModel
{
    public string DisplayName { get; init; } = string.Empty;
    public string Biography { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}