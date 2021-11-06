using Photobook.Notifications.Models;

namespace Photobook.Notifications.Parser
{
    public interface IParser
    {
        string Parse(string template, IEmailNotification notification);
    }
}
