using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.ComponentModel;

namespace Mvc.Mailer {
    public class TestSmtpClient : SmtpClientBase {
        private static List<MailMessage> _sentMails;
        public static List<MailMessage> SentMails {
            get {
                _sentMails = _sentMails ?? new List<MailMessage>();
                return _sentMails;
            }
        }

        public static bool WasLastCallAsync { get; set; }

        public override void Send(MailMessage mailMessage) {
            WasLastCallAsync = false;
            SentMails.Add(mailMessage);
        }

        public override void SendAsync(MailMessage mailMessage, object userState) {
            WasLastCallAsync = true;
            SentMails.Add(mailMessage);
            OnSendCompleted(this, new AsyncCompletedEventArgs(null, false, userState));
        }

        public override void Dispose() {
        }
    }
}