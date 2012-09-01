using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Mvc.Mailer
{
    public abstract class SmtpClientBase : ISmtpClient
    {
        public event SendCompletedEventHandler SendCompleted;

        protected void OnSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SendCompleted != null)
            {
                SendCompleted(sender, e);
            }
        }

        public abstract void Send(MailMessage mailMessage);

        public virtual void SendAsync(MailMessage mailMessage)
        {
            var userState = "userState";
            SendAsync(mailMessage, userState);
        }

        public abstract void SendAsync(MailMessage mailMessage, object userState);
    	public abstract Task SendTaskAsync(MailMessage mailMessage);

    	public abstract void Dispose();
    }
}
