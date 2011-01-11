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
	/// using Mvc.Mailer;
    /// new Notifier().WelcomeMessage().Send();
	/// The web.config file section for mails is already added to your project.
	/// Just edit your web.config file mailSettings and provide with required server, port, user, password etc.
    /// </summary>
	public class Notifier : MailerBase
	{
		/// <summary>
        /// In your constructor you can specify a default MasterName
        /// or a Default mime type to Html or Text
        /// </summary>
        public Notifier()
        {
            this.MasterName = "~/Views/Notifier/_Layout.cshtml";
            this.IsBodyHtml = true;
        }

        /// <summary>
        /// Gnereate a welcome message
        /// </summary>
        /// <returns></returns>
        public MailMessage WelcomeMessage()
        {
            //Create a MailMessage object
            var mailMessage = new MailMessage { Subject = "Testing by Sohan" };

            //cretae an instance of viewData if you need StronglyTyped views or any ViewData
            //var viewData = new ViewDataDictionary<Address>
            //{
            //    Model = new Address { City = "Calgary", Province = "AB", Street = "600 6th Ave NW" }
            //};
            
            //Get the EmailBody - this will be the string containing the view after it has been rendered by your ViewEngine
            mailMessage.Body = PopulateBody(mailMessage: mailMessage, 
                                            viewName: "~/Views/Notifier/WelcomeMessage.cshtml" 
                                            //,viewData: viewData
                                            // Uncomment the following to Customize the masterName for this message
                                            //,masterName: "~/Views/Mailer/_AnotherLayout.cshtml"
                                            );
            
            //Uncomment the following if you want to send a Text/Plain email instead
            //mailMessage.IsBodyHtml = false;

            //Add one/more Recipient(s)
            mailMessage.To.Add("some-email@gmail.com");

            //Edit the following line to Add an Attachment
            //mailMessage.Attachments.Add(new Attachment("SitePolicy.pdf"));
            
            return mailMessage;
        }
	}
}
