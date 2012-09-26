using System;
using System.Net.Mail;

namespace Mvc.Mailer {
    public interface ISmtpClient : IDisposable {
        event SendCompletedEventHandler SendCompleted;
        void Send(MailMessage mailMessage);
        void SendAsync(MailMessage mailMessage);
        void SendAsync(MailMessage mailMessage, object userState);
    }
}
