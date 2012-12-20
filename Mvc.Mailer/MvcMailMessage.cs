﻿using System.Collections.Generic;
using System.Net.Mail;

namespace Mvc.Mailer {
    public class MvcMailMessage : MailMessage {
        public string ViewName { get; set; }
        public string MasterName { get; set; }
        public Dictionary<string,string> LinkedResources { get; set; }

        /// <summary>
        /// Sends a MailMessage using smtpClient
        /// </summary>
        /// <param name="smtpClient">leave null to use default System.Net.Mail.SmtpClient</param>
        public virtual void Send(ISmtpClient smtpClient = null)
        {
            if (To.Count == 0)
                return;
            smtpClient = smtpClient ?? GetSmtpClient();
            using (smtpClient)
            {
                smtpClient.Send(this);
            }
        }

        /// <summary>
        /// Asynchronously Sends a MailMessage using smtpClient
        /// </summary>
        /// <param name="userState">The userState</param>
        /// <param name="smtpClient">leave null to use default System.Net.Mail.SmtpClient</param>
        public virtual void SendAsync(object userState = null, ISmtpClient smtpClient = null)
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
