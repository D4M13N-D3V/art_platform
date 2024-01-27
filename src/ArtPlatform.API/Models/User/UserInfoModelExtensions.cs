namespace ArtPlatform.API.Models.User;

public static class UserInfoModelExtensions
{
    public static UserInfoModel ToModel(this Database.Entities.User user)
    {
        return new()
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Biography = user.Biography,
            Email = user.Email
        };
    }
    public static Database.Entities.User ToEntity(this UserInfoUpdateModel user, Database.Entities.User existingUser)
    {
        existingUser.DisplayName = user.DisplayName;
        existingUser.Biography = user.Biography;
        existingUser.Email = user.Email;
        return existingUser;
    }
}