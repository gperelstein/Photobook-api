namespace Photobook.Common.Configuration
{
    public class AppOptions
    {
        public const string AppConfiguration = "AppConfiguration";
        public SmtpOptions Smtp { get; set; }
        public UrlsOptions Urls { get; set; }
    }
}
