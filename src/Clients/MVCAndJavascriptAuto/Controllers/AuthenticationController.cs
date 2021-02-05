using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties()
            {
                RedirectUri = "../logout.html"
            },new string[] { "oidc", "Cookies" });
    }
}
