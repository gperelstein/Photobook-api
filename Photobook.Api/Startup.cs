using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Photobook.Common.Configuration;
using Photobook.Data;
using Photobook.Logic;
using Photobook.Notifications;
using System.Collections.Generic;
using System.IO;

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
            services.AddLogic(Configuration);
            services.AddNotifications();
            services.AddHttpContextAccessor();
            services.AddControllers();

            services.AddAuthorization();

            var urlsOptions = Configuration
                .GetSection("AppConfiguration:Urls")
                .Get<UrlsOptions>();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(x =>
                {
                    x.Authority = urlsOptions.IdentityServerUrl; //idp address
                    x.RequireHttpsMetadata = false;
                    x.ApiName = "photobookweb";
                    x.ApiSecret = "secret";
                });

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
                        Password = new NSwag.OpenApiOAuthFlow
                        {
                            TokenUrl = "https://localhost:5001/connect/token",
                            Scopes = new Dictionary<string, string> { { "write", "Demo API - full access" } }
                        }
                    },
                    In = OpenApiSecurityApiKeyLocation.Header
                });
                options.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
            });
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
            services.AddControllers().AddFluentValidation();
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
                        AppName = "photobookweb",
                        ClientId = "service.client",
                        ClientSecret = "secret",
                        UsePkceWithAuthorizationCodeGrant = true
                    };
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Images")),
                RequestPath = "/Images"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
