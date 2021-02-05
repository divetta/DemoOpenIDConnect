using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCAndJavascriptAuto.Model;
using System.Linq;
using System.Threading.Tasks;

namespace MVCAndJavascriptAuto.Controllers
{
    [Route("identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAsync()
        {
            var user = new User()
            {
                Name = User.Claims.Where(c => c.Type == "name").First().Value,
                GivenName = User.Claims.Where(c => c.Type == "given_name").First().Value,
                FamilyName = User.Claims.Where(c => c.Type == "family_name").First().Value,
                Email = User.Claims.Where(c => c.Type == "email").First().Value,
                WebSite = User.Claims.Where(c => c.Type == "website").First().Value,
            };

            return new JsonResult(user);
        }

        [HttpGet]
        [Route("isauthenticated")]
        public IActionResult IsAuthenticated()
        {
            return new JsonResult(User.Identity.IsAuthenticated);
        }
    }
}
