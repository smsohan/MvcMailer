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
        public static void Send(this MailMessage message)
        {
            SmtpClient mailClient = new SmtpClient();
            mailClient.Send(message);
        }

        public static void SendAsync(this MailMessage message)
        {
            SmtpClient mailClient = new SmtpClient();
            var userState = "nothing";
            mailClient.SendAsync(message, userState);
        }
    }

}