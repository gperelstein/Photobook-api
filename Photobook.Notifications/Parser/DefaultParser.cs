using Photobook.Notifications.Models;
using System.Linq;

namespace Photobook.Notifications.Parser
{
    public class DefaultParser : IParser
    {
        public string Parse(string template, IEmailNotification notification)
        {
            if (template == null)
            {
                return null;
            }

            if (notification == null)
            {
                return template;
            }

            var type = notification.GetType();
            var parsedTemplate = template;
            foreach (var property in type.GetProperties()
                .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string)))
            {
                var value = property.GetValue(notification)?.ToString() ?? string.Empty;
                parsedTemplate = parsedTemplate.Replace($"{{{{{property.Name}}}}}", value);
            }

            return parsedTemplate;
        }
    }
}
