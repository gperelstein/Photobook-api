using Photobook.Notifications.Models;
using System.Threading.Tasks;

namespace Photobook.Notifications.TemplateLocator
{
    public interface ITemplateLocator
    {
        Task<string> LocateTemplateAsync(IEmailNotification notification);
    }
}
