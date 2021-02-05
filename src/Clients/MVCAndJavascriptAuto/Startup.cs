using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MVCAndJavascriptAuto.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVCAndJavascriptAuto
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHttpClient();

            services.AddOcelot().AddDelegatingHandler<MyTokenHandler>(true);

            services.AddSingleton<IDiscoveryCache>(r =>
            {
                var factory = r.GetRequiredService<IHttpClientFactory>();
                return new DiscoveryCache(MyConstants.Authority, () => factory.CreateClient());
            });


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie(options =>
                {
                    options.Cookie.Name = "oidc";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = MyConstants.Authority;
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "mvc-and-js-auto";
                    options.ClientSecret = "super-secret";

                    options.ResponseType = "code";
                    options.UsePkce = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("address");
                    options.Scope.Add("website");
                    options.Scope.Add("api1");

                    options.ClaimActions.MapAll();

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization();
            });

            app.UseOcelot().Wait();
        }
    }
}
