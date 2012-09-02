using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Mvc.Mailer
{
    public class TestSmtpClient : SmtpClientBase
    {
    	private static object lockObject;

        private static List<MailMessage> _sentMails;
        public static List<MailMessage> SentMails
        {
            get{
                _sentMails = _sentMails ?? new List<MailMessage>();
                return _sentMails;
            }
        }

        public static bool WasLastCallAsync
        {
            get;
            set;
        }

        public TestSmtpClient()
        {
			lockObject = new object();
        }

        public override void Send(MailMessage mailMessage)
        {
            WasLastCallAsync = false;
            SentMails.Add(mailMessage);
        }

        public override void SendAsync(MailMessage mailMessage, object userState)
        {
            WasLastCallAsync = true;

        	Task.Factory.StartNew(() =>
        	                      	{
										lock (lockObject)
										{
											SentMails.Add(mailMessage);
										}
        	                      	}).
        		ContinueWith(task =>
        		             	{
        		             		OnSendCompleted(this, new AsyncCompletedEventArgs(null, false, userState));
        		             	});
        }

    	public override Task SendTaskAsync(MailMessage mailMessage)
    	{
    		WasLastCallAsync = true;
			
    		return Task.Factory.StartNew(() =>
    		                             	{
    		                             		lock (lockObject)
    		                             		{
    		                             			SentMails.Add(mailMessage);
    		                             		}
    		                             	});
    	}

    	public override void Dispose()
        {

        }
    }
}
