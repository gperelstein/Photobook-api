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

namespace Photobook.Logic
{
    public static class ServicesConfigurator
    {
        public static IServiceCollection AddLogic(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddDefaultIdentity<PhotobookUser>()
                    .AddRoles<PhotobookRole>()
                    .AddEntityFrameworkStores<PhotobookDbContext>()
                    .AddDefaultTokenProviders()
                    .AddPasswordValidator<PasswordValidator>();

            services.AddIdentityServer(options =>
            {
                options.UserInteraction = new UserInteractionOptions()
                {
                    LogoutUrl = "http://localhost:3000/login",
                    LoginUrl = "http://localhost:3000/success",
                    ErrorUrl = "http://localhost:3000/error"
                };
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

            return services;
        }
    }
}
