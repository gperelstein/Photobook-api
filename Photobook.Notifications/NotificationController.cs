using Photobook.Notifications.Client;
using Photobook.Notifications.Models;
using Photobook.Notifications.Parser;
using Photobook.Notifications.TemplateLocator;
using System.Threading.Tasks;

namespace Photobook.Notifications
{
    public class NotificationController : INotificationController
    {
        private readonly ITemplateLocator _locator;
        private readonly IParser _parser;
        private readonly ISmtpProvider _provider;

        public NotificationController(ITemplateLocator locator, IParser parser, ISmtpProvider provider)
        {
            _locator = locator;
            _parser = parser;
            _provider = provider;
        }

        public async Task<bool> PushAsync(EmailNotification notification)
        {
            if (notification == null)
            {
                return false;
            }

            var template = await _locator.LocateTemplateAsync(notification);

            if (template == null)
            {
                return false;
            }

            notification.ParsedResult = _parser.Parse(template, notification);

            return await _provider.SendNotificationAsync(notification);
        }
    }
}
