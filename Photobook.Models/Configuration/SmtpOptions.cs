namespace Photobook.Models.Configuration
{
    public class SmtpOptions
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool EnableSsl { get; set; }
    }
}
