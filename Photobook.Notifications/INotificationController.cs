using Photobook.Notifications.Models;
using System.Threading.Tasks;

namespace Photobook.Notifications
{
    public interface INotificationController
    {
        Task<bool> PushAsync(EmailNotification notification);
    }
}
