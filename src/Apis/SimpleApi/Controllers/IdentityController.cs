using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace SimpleApi.Controllers
{
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();

            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = MyConstants.Authority + "/connect/userinfo",
                Token = token
            });

            if (response.IsError) throw new Exception(response.Error);

            var claims = response.Claims;


            return new JsonResult(from c in response.Claims select new { c.Type, c.Value });
        }
    }
}
