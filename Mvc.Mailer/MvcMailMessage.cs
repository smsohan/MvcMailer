using System.Collections.Generic;
using System.Net.Mail;

namespace Mvc.Mailer {
    public class MvcMailMessage : MailMessage {
        public string ViewName { get; set; }
        public string MasterName { get; set; }
        public Dictionary<string,string> LinkedResources { get; set; }
    }
}
