using System.Security.Claims;

namespace comissions.app.api.Extensions;

public static class UserExtension
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}