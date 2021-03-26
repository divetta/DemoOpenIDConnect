using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Web;
using System.Web.UI;

namespace WebFormsClient
{
    public class AuthenticatedPage : Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Context.GetOwinContext().Authentication.Challenge(OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }

            base.OnPreLoad(e);
        }
    }
}