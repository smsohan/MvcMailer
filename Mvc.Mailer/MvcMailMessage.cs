using System.Collections.Generic;
using System.Net.Mail;

namespace Mvc.Mailer {
    public class MvcMailMessage : MailMessage {
        public string ViewName { get; set; }
        public string MasterName { get; set; }
        public Dictionary<string,string> LinkedResources { get; set; }

        /// <summary>
        /// Sends a MailMessage using the default System.Net.Mail.SmtpClient
        /// </summary>
        public virtual void Send()
        {
            Send(null);
        }

        /// <summary>
        /// Sends a MailMessage
        /// </summary>
        /// <param name="smtpClient">The System.Net.Mail.SmtpClient to use</param>
        public virtual void Send(ISmtpClient smtpClient)
        {
            smtpClient = smtpClient ?? GetSmtpClient();
            using (smtpClient)
            {
                smtpClient.Send(this);
            }
        }

        /// <summary>
        /// Asynchronously Sends a MailMessage the default System.Net.Mail.SmtpClient
        /// </summary>
        public virtual void SendAsync()
        {
            SendAsync(null, null);
        }

        /// <summary>
        /// Asynchronously Sends a MailMessage
        /// </summary>
        /// <param name="smtpClient">The System.Net.Mail.SmtpClient to use</param>
        public virtual void SendAsync(ISmtpClient smtpClient)
        {
            SendAsync(null, smtpClient);
        }

        /// <summary>
        /// Asynchronously Sends a MailMessage using the default System.Net.Mail.SmtpClient
        /// </summary>
        /// <param name="userState">The userState</param>
        public virtual void SendAsync(object userState = null)
        {
            SendAsync(userState, null);
        }

        /// <summary>
        /// Asynchronously Sends a MailMessage
        /// </summary>
        /// <param name="userState">The userState</param>
        /// <param name="smtpClient">The System.Net.Mail.SmtpClient to use</param>
        public virtual void SendAsync(object userState, ISmtpClient smtpClient)
        {
            smtpClient = smtpClient ?? GetSmtpClient();
            smtpClient.SendAsync(this, userState);
        }

        public virtual ISmtpClient GetSmtpClient()
        {
            if (MailerBase.IsTestModeEnabled)
            {
                return new TestSmtpClient();
            }
            return new SmtpClientWrapper();
        }
    }
}
