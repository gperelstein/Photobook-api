using Photobook.Notifications.Models;
using System.Threading.Tasks;

namespace Photobook.Notifications.Client
{
    public interface ISmtpProvider
    {
        Task<bool> SendNotificationAsync(IEmailNotification notification);
    }
}