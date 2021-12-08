using Photobook.Notifications.Models;

namespace Photobook.Notifications.Templates
{
    public class InitiatePasswordResetNotification : EmailNotification
    {
        public override string TemplateName => "InitiatePasswordResetNotification.html";
        public string ResetPasswordLink { get; set; }
        public string FirstName { get; set; }
    }
}
