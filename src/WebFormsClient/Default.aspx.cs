using System;
using System.Web;

namespace WebFormsClient
{
    public partial class Default : AuthenticatedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                foreach (var claim in Context.GetOwinContext().Authentication.User.Claims)
                {
                    Response.Write(claim.Type + ": " + claim.Value + "<br>");
                }
            }
        }
    }
}