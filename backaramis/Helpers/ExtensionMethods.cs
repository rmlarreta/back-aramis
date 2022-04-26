using System.Security.Claims;

namespace backaramis.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// User ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// 

        public static string GetUserName(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }

            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value + ' ' + currentUser.FindFirst(ClaimTypes.GivenName).Value;
        }

        public static string GetUserPerfil(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }

            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.Role).Value;
        }
        //aramis
    }
}
