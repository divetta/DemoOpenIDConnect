using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MVCAndJavascriptAuto.Helpers;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;

namespace MVCAndJavascriptAuto
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationHelper.Instance.PropagateConfigurationAsync(configuration).Wait();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAccessTokenManagement();
            
            services.AddControllers();
            services.AddDistributedMemoryCache();

            services.AddOcelot().AddDelegatingHandler<OcelotDelegatingHandler>(true);

            services.AddDataProtection().SetApplicationName("MVCAndJavascriptAuto");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options = CookieAuthenticationOptionsFactory.Instance.GetDefaultOptions(options);
            })
            .AddOpenIdConnect(options =>
            {
                options = OidcOptionsFactory.Instance.GetDefaultOptions(options);

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");

                options.Scope.Add("authorization_group");
                options.Scope.Add("entitlement_group");

                options.CallbackPath = new PathString("/signin-oidc");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            //app.UseMiddleware<StrictSameSiteExternalAuthenticationMiddleware>();
            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                logger.LogInformation($"IsAuthenticated? {context.User.Identity.IsAuthenticated}");

                if (context.User.Identity.IsAuthenticated)
                {
                    foreach (var claim in context.User.Claims)
                    {
                        logger.LogInformation($"{claim.Type}: {claim.Value}");
                    }
                }


                if (!context.User.Identity.IsAuthenticated && context.Request.Path.Value == "/")
                {
                    await context.ChallengeAsync();
                    return;
                }

                await next();
            });

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization();
            });

            app.UseOcelot().Wait();
        }
    }
}
