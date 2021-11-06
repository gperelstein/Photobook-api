namespace Photobook.Models.Configuration
{
    public class AppOptions
    {
        public const string AppConfiguration = "AppConfiguration";
        public SmtpOptions Smtp { get; set; }
    }
}
