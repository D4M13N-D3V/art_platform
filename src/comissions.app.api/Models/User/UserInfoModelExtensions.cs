namespace comissions.app.api.Models.User;

public static class UserInfoModelExtensions
{
    public static UserInfoModel ToModel(this database.Entities.User user)
    {
        return new()
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Biography = user.Biography,
            Email = user.Email
        };
    }
    public static database.Entities.User ToEntity(this UserInfoUpdateModel user, database.Entities.User existingUser)
    {
        existingUser.DisplayName = user.DisplayName;
        existingUser.Biography = user.Biography;
        return existingUser;
    }
}