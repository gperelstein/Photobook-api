using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Photobook.MigrationUtility
{
    public class MigrationUtilityStartup
    {
        public MigrationUtilityStartup()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("LearningAnalyticsAPIContext"));
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}