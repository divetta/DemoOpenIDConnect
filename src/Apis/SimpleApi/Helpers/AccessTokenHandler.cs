using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleApi.Helpers
{
    public class AccessTokenHandler
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public AccessTokenHandler(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var token = await httpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(token))
            {
                var cachedUserInfo = await _cache.GetAsync(token);

                if (cachedUserInfo != null)
                {
                    var claims = JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(cachedUserInfo));
                    httpContext.Items["UserClaims"] = claims;
                }
                else
                {
                    var client = new HttpClient();

                    var response = await client.GetUserInfoAsync(new UserInfoRequest
                    {
                        Address = ConfigurationHelper.Instance.UserInfoEndpoint,
                        Token = token
                    });

                    if (response.IsError) throw new Exception(response.Error);

                    var claims = response.Claims.Where(c => c.Type == "entitlement_group").Select(c => c.Value).ToList();

                    var options = new DistributedCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                    var output = JsonConvert.SerializeObject(claims, Formatting.Indented);
                    byte[] encodedUserClaims = Encoding.UTF8.GetBytes(output);

                    await _cache.SetAsync(token, encodedUserClaims, options);

                    httpContext.Items["UserClaims"] = claims;
                }

            }

            await _next(httpContext);
        }
    }
}
