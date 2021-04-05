using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SimpleApi.Controllers
{
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var claims = (List<string>)HttpContext.Items["UserClaims"];
            return new JsonResult(claims);
        }
    }
}
