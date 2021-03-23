using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVCAndJavascriptAuto.Controllers
{
    [Authorize]
    public class AuthenticationController : Controller
    {
        public void Login()
        {
            Response.Redirect("../");
        }

        public IActionResult Logout() =>
            SignOut(new AuthenticationProperties()
            { RedirectUri = "../logout.html" }, new string[] { "oidc", "Cookies" });
    }
}
