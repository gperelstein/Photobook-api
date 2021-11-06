using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photobook.Notifications.Models
{
    public abstract class EmailNotification : IEmailNotification
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string ParsedResult { get; set; }
        public abstract string TemplateName { get; }
    }
}
