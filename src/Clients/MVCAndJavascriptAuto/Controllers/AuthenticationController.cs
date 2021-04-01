using Microsoft.AspNetCore.Mvc;

namespace MVCAndJavascriptAuto.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Logout() =>
            SignOut(new string[] { "oidc", "Cookies" });
    }
}
