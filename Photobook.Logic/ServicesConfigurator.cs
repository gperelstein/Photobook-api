using FluentValidation;
using IdentityServer4.AspNetIdentity;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Photobook.Common.Configuration;
using Photobook.Common.Identity;
using Photobook.Common.Services.CurrentUser;
using Photobook.Common.Services.Files;
using Photobook.Data;
using Photobook.Logic.Identity;
using Photobook.Logic.Validators;
using System.IO.Abstractions;
using System.Reflection;

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
            var identityOptions = configuration
                .GetSection("AppConfiguration:Identity")
                .Get<Common.Configuration.IdentityOptions>();
            var urlsOptions = configuration
                .GetSection("AppConfiguration:Urls")
                .Get<UrlsOptions>();
            services.AddIdentityServer(options =>
            {
                options.IssuerUri = urlsOptions.IdentityServerUrl;
            })
            .AddInMemoryClients(Config.GetClients(identityOptions))
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApis(identityOptions))
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
