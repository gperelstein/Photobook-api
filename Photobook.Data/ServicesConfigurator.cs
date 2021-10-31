using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Photobook.Data
{
    public static class ServicesConfigurator
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PhotobookDbContext>(c =>
            {
                c.UseSqlServer(configuration.GetConnectionString("PhotobookSqlServer"));
            });

            return services;
        }
    }
}
