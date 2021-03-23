using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace SimpleApi.Helpers
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { claimValue };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly string _claim;

        public ClaimRequirementFilter(string claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var claims = (List<string>)context.HttpContext.Items["UserClaims"];

            var hasClaim = claims.Any(c => c == _claim);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
