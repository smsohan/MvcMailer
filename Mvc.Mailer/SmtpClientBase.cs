using System.Net.Mail;
using System.ComponentModel;

namespace Mvc.Mailer {
    public abstract class SmtpClientBase : ISmtpClient {
        public event SendCompletedEventHandler SendCompleted;

        protected void OnSendCompleted(object sender, AsyncCompletedEventArgs e) {
            if (SendCompleted != null) {
                SendCompleted(sender, e);
            }
        }

        public abstract void Send(MailMessage mailMessage);

        public virtual void SendAsync(MailMessage mailMessage) {
            const string userState = "userState";
            SendAsync(mailMessage, userState);
        }

        public abstract void SendAsync(MailMessage mailMessage, object userState);
        public abstract void Dispose();
    }
}
