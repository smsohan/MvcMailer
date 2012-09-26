using System.Net.Mail;

namespace Mvc.Mailer {
    /// <summary>
    /// Adds the much needed send method to MailMessage so that you can do the following
    /// MailMessage email = new MyMailer().WelcomeMessage();
    /// email.Send();
    /// 
    /// The underlying implementation utilizes the SMTPClient class to send the emails.
    /// </summary>
    public static class MailMessageExtensions {
        /// <summary>
        /// Sends a MailMessage using smtpClient
        /// </summary>
        /// <param name="message">The mailMessage Object</param>
        /// <param name="smtpClient">leave null to use default System.Net.Mail.SmtpClient</param>
        public static void Send(this MailMessage message, ISmtpClient smtpClient = null) {
            smtpClient = smtpClient ?? GetSmtpClient();
            using (smtpClient) {
                smtpClient.Send(message);
            }
        }

        /// <summary>
        /// Asynchronously Sends a MailMessage using smtpClient
        /// </summary>
        /// <param name="message">The mailMessage Object</param>
        /// <param name="userState">The userState</param>
        /// <param name="smtpClient">leave null to use default System.Net.Mail.SmtpClient</param>
        public static void SendAsync(this MailMessage message, object userState = null, ISmtpClient smtpClient = null) {
            smtpClient = smtpClient ?? GetSmtpClient();
            smtpClient.SendAsync(message, userState);
        }

        public static ISmtpClient GetSmtpClient() {
            if (MailerBase.IsTestModeEnabled) {
                return new TestSmtpClient();
            }
            return new SmtpClientWrapper();
        }
    }
}