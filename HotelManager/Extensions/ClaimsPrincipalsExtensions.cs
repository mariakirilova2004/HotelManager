using System.Security.Claims;

namespace HotelManager.Extensions
{
    public static class ClaimsPrincipalsExtensions
    {
        public static string Id(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin"); ;
        }
    }
}
