using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace MVCAndJavascriptAuto.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Logout() =>
            SignOut(new string[] { OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme });
    }
}
