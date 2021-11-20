using Photobook.Notifications.Models;

namespace Photobook.Notifications.Templates
{
    public class UserRegistration : EmailNotification
    {
        public override string TemplateName => "UserRegistration.html";
        public string RegistrationLink { get; set; }
        public string FirstName { get; set; }
    }
}
