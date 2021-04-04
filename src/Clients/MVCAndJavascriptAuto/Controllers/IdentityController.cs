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
            return new JsonResult(User.Claims.Select(c => $"{c.Type}: {c.Value}"));
        }
    }
}
