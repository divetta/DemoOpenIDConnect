using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MVCAndJavascriptAuto.Controllers
{
    [Route("identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(new {
                Name = User.Claims.FirstOrDefault(c => c.Type == "name").Value,
                GivenName = User.Claims.FirstOrDefault(c => c.Type == "given_name").Value,
                FamilyName = User.Claims.FirstOrDefault(c => c.Type == "family_name").Value,
                Email = User.Claims.FirstOrDefault(c => c.Type == "email").Value
            });
        }
    }
}
