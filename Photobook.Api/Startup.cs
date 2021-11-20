using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Photobook.Data;
using Photobook.Logic;
using Photobook.Common.Configuration;
using Photobook.Notifications;
using System;
using System.Collections.Generic;

namespace Photobook.Api
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
            services.Configure<AppOptions>(Configuration.GetSection(AppOptions.AppConfiguration));
            services.AddData(Configuration);
            services.AddLogic();
            services.AddNotifications();
            services.AddHttpContextAccessor();
            services.AddControllers();

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    policy => { policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
            });
            services.AddOpenApiDocument(options =>
            {
                options.DocumentName = "v1";
                options.Title = "Protected API";
                options.Version = "v1";

                options.AddSecurity("oauth2", new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Flows = new NSwag.OpenApiOAuthFlows
                    {
                        AuthorizationCode = new NSwag.OpenApiOAuthFlow
                        {
                            AuthorizationUrl = "https://localhost:5001/connect/authorize",
                            TokenUrl = "https://localhost:5001/connect/token",
                            Scopes = new Dictionary<string, string> { { "openid", "Demo API - full access" } }
                        }
                    }
                });
                options.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3(settings =>
                {
                    settings.OAuth2Client = new OAuth2ClientSettings
                    {
                        AppName = "Demo API - Swagger",
                        ClientId = "service.client",
                        ClientSecret = "secret",
                        UsePkceWithAuthorizationCodeGrant = true
                    };
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
