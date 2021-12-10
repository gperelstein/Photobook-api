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
            var identityOptions = Configuration
                .GetSection("AppConfiguration:Identity")
                .Get<IdentityOptions>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(x =>
                {
                    x.Authority = urlsOptions.IdentityServerUrl; //idp address
                    x.RequireHttpsMetadata = false;
                    x.ApiName = identityOptions.ClientName;
                    x.ApiSecret = identityOptions.ClientSecret;
                });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    policy => { policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
            });
            services.AddOpenApiDocument(options =>
            {
                options.DocumentName = "v1";
                options.Title = "Photobook Api";
                options.Version = "v1";
                options.AddSecurity("oauth2", new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Flows = new NSwag.OpenApiOAuthFlows
                    {
                        Password = new NSwag.OpenApiOAuthFlow
                        {
                            TokenUrl = $"{urlsOptions.TokenUrl}/connect/token",
                            Scopes = new Dictionary<string, string>
                            {
                                { "regular", "Regular user scope" }
                            }
                        }
                    },
                    In = OpenApiSecurityApiKeyLocation.Header
                });
                options.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
            });
            services.AddControllers().AddFluentValidation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var identityOptions = Configuration
                .GetSection("AppConfiguration:Identity")
                .Get<IdentityOptions>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3(settings =>
                {
                    settings.OAuth2Client = new OAuth2ClientSettings
                    {
                        AppName = identityOptions.ClientName,
                        ClientId = identityOptions.ClientId,
                        ClientSecret = identityOptions.ClientSecret,
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

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
