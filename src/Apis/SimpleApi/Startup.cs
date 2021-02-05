using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace SimpleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddDistributedMemoryCache();

            services.AddAuthentication("token")
                // JWT tokens
                .AddJwtBearer("token", options =>
                {
                    options.Authority = MyConstants.Authority;

                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                })
                
                // reference tokens
                .AddOAuth2Introspection("introspection", options =>
                {
                    options.Authority = MyConstants.Authority;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(15);
                    options.SaveToken = true;

                    options.ClientId = "simple-api";
                    options.ClientSecret = "secret";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api1"); 
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:44304", "https://localhost:44305")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization("ApiScope"); 
            });
        }
    }
}
