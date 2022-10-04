using System.Security.Claims;

namespace backaramis.Helpers
{
    public class SecurityService
    {
        private readonly HttpContext _hcontext;
        private readonly ClaimsPrincipal _cp;
        public SecurityService(IHttpContextAccessor haccess)
        {
            _hcontext = haccess.HttpContext!;
            _cp = _hcontext.User;
        }

        public string GetUserAuthenticated()
        {
            string? user = ExtensionMethods.GetUserName(_cp);
            if (user == null)
            {
                return null!;
            }

            return user;
        }

        public int GetPerfilAuthenticated()
        {
            string? perfil = ExtensionMethods.GetUserPerfil(_cp);
            if (perfil == null)
            {
                return 0;
            }

            return Convert.ToInt32(perfil);
        }

    }
}
