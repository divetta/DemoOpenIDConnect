using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace MVCAndJavascriptAuto.Controllers
{
    [Route("identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {

        readonly ILogger<IdentityController> _logger;

        public IdentityController(ILogger<IdentityController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(User.Claims.Select(c => $"{c.Type}: {c.Value}"));
        }
    }
}
