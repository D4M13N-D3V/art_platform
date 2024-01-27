using System.Security.Claims;

namespace ArtPlatform.API.Extensions;

public static class UserExtension
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}