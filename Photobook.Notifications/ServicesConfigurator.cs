using Microsoft.Extensions.DependencyInjection;
using Photobook.Notifications.Client;
using Photobook.Notifications.Parser;
using Photobook.Notifications.TemplateLocator;

namespace Photobook.Notifications
{
    public static class ServicesConfigurator
    {
        public static IServiceCollection AddNotifications(this IServiceCollection services)
        {
            services.AddTransient<IParser, DefaultParser>();
            services.AddTransient<ISmtpProvider, SmtpProvider>();
            services.AddTransient<ITemplateLocator, DefaultTemplateLocator>();
            services.AddTransient<INotificationController, NotificationController>();

            return services;
        }
    }
}
