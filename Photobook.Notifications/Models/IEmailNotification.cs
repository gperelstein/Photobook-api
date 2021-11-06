namespace Photobook.Notifications.Models
{
    public interface IEmailNotification
    {
        public string TemplateName { get; }
        public string ParsedResult { get; set; }
    }
}
