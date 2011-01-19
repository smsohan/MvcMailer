using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Mvc.Mailer
{
    /// <summary>
    /// Adds the much needed send method to MailMessage so that you can do the following
    /// MailMessage email = new MyMailer().WelcomeMessage();
    /// email.Send();
    /// 
    /// The underlying implementation utilizes the SMTPClient class to send the emails.
    /// </summary>
    public static class MailMessageExtensions
    {

        /// <summary>
        /// Sends a MailMessage using smtpClient
        /// </summary>
        /// <param name="message">The mailMessage Object</param>
        /// <param name="smtpClient">leave null to use default System.Net.Mail.SmtpClient</param>
        public static void Send(this MailMessage message, SmtpClient smtpClient = null)
        {
            smtpClient = smtpClient ?? new SmtpClient();
            smtpClient.Send(message);
        }

        /// <summary>
        /// Asynchronously Sends a MailMessage using smtpClient
        /// </summary>
        /// <param name="message">The mailMessage Object</param>
        /// <param name="smtpClient">leave null to use default System.Net.Mail.SmtpClient</param>
        public static void SendAsync(this MailMessage message, SmtpClient smtpClient = null)
        {
            smtpClient = smtpClient ?? new SmtpClient();
            var userState = "nothing";
            smtpClient.SendAsync(message, userState);
        }
    }

}