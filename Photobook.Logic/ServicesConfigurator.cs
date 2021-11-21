using IdentityServer4.AspNetIdentity;
using IdentityServer4.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Photobook.Data;
using Photobook.Logic.Identity;
using Photobook.Logic.Validators;
using Photobook.Common.Identity;
using System.Reflection;
using System.IO;
using Photobook.Common.Services;
using Photobook.Common.Services.Files;
using System.IO.Abstractions;
using Microsoft.Extensions.Configuration;
using Photobook.Common.Configuration;
using FluentValidation;

namespace Photobook.Logic
{
    public static class ServicesConfigurator
    {
        public static IServiceCollection AddLogic(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddDefaultIdentity<PhotobookUser>()
                    .AddRoles<PhotobookRole>()
                    .AddEntityFrameworkStores<PhotobookDbContext>()
                    .AddDefaultTokenProviders()
                    .AddPasswordValidator<PasswordValidator>();

            var urlsOptions = configuration
                .GetSection("AppConfiguration:Urls")
                .Get<UrlsOptions>();
            services.AddIdentityServer(options =>
            {
                options.IssuerUri = urlsOptions.IdentityServerUrl;
            })
            .AddInMemoryClients(Config.GetClients())
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApis())
            .AddInMemoryApiScopes(Config.GetScopes())
            .AddProfileService<ProfileService<PhotobookUser>>()
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            .AddDeveloperSigningCredential();

            services.AddAuthentication()
                    .AddIdentityServerJwt();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
            });

            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IFilesService, ProfilePicturesService>();
            services.AddTransient<IFileSystem, FileSystem>();

            return services;
        }
    }
}
