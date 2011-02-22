using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using Mvc.Mailer;

namespace $rootnamespace$.Mailers
{
	/// <summary>
    /// To Send a welcome message do the following:
	///<code>
	/// using Mvc.Mailer;
	/// 
    /// new Notifier().WelcomeMessage().Send();
	///</code>
	/// The web.config file section for mails is already added to your project.
	/// Just edit your web.config file mailSettings and provide with required server, port, user, password etc.
    /// </summary>
	public class Notifier : MailerBase, INotifier
	{
		/// <summary>
        /// In your constructor you can specify a default MasterName
        /// or a Default mime type to Html or Text
        /// </summary>
        public Notifier()
        {
            this.MasterName = "_Layout";
            this.IsBodyHtml = true;
        }

        /// <summary>
        /// Gnereate a welcome message
        /// </summary>
        /// <returns></returns>
        public MailMessage WelcomeMessage()
        {
            var mailMessage = new MailMessage { Subject = "Welcome to MvcMailer" };
			ViewBag.Name = "Sohan";
			PopulateBody(mailMessage, "WelcomeMessage");
			mailMessage.To.Add("some-email@gmail.com");
            return mailMessage;
        }
	}
}
