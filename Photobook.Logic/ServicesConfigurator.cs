using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Photobook.Data;
using Photobook.Models.Identity;
using System.Reflection;

namespace Photobook.Logic
{
    public static class ServicesConfigurator
    {
        public static IServiceCollection AddLogic(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services
                .AddDefaultIdentity<PhotobookUser>()
                .AddRoles<PhotobookRole>()
                .AddEntityFrameworkStores<PhotobookDbContext>();

            services.AddIdentityServer().AddApiAuthorization<PhotobookUser, PhotobookDbContext>();

            services.AddAuthentication().AddIdentityServerJwt();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
            });

            return services;
        }
    }
}
