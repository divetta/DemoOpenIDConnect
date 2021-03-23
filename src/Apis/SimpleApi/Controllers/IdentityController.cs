using Microsoft.AspNetCore.Mvc;
using SimpleApi.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleApi.Controllers
{
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        [ClaimRequirement("BMBVU.Bens-Imoveis-Escrita")]
        public async Task<IActionResult> GetAsync()
        {
            var claims = (List<string>)HttpContext.Items["UserClaims"];
            return new JsonResult(claims);
        }
    }
}
