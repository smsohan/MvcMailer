using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Mvc.Html;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.Routing;

namespace Mvc.Mailer
{
    /// <summary>
    /// The Base class for Mailers. Your mailer should subclass:
    /// 
    /// public clas MyMailer : Mvc.Mailer.MailerBase{
    ///     public MailMessage WelcomeMessage(User newUser){
    ///         MailMessage mailMessage = new MailMessage{Subject = "Welcome to TheBestEverSite.com"};
    ///         mailMessage.To.Add(newUser.EmailAddress);
    ///         mailMessage.Body = EmailBody("~/Views/MyMailer/WelcomeMessage.cshtml");
    ///         return mailMessage;
    ///     }
    /// }
    /// 
    /// </summary>
    public class MailerBase : ControllerBase
    {
        /// <summary>
        /// The parameterless constructor
        /// </summary>
        public MailerBase()
        {
            
        }

        /// <summary>
        /// The Path to the MasterPage or Layout
        /// myMailer.MasterName = "~/Views/MyMailer/_Layout.cshtml"
        /// </summary>
        public string MasterName
        {
            get;
            set;
        }

        public bool IsBodyHtml
        {
            get;
            set;
        }

        /// <summary>
        /// Nothing to Execute at this point, left blank
        /// </summary>
        protected override void ExecuteCore()
        {
           
        }

        /// <summary>
        /// This method generates the EmailBody from the given viewName, masterName and viewData
        /// </summary>
        /// <param name="viewName">@example: "~/Views/MyMailer/WelcomeMessage.cshtml" </param>
        /// <param name="masterName">@example: "~/Views/MyMailer/_Layout.cshtml"</param>
        /// <param name="viewData">>@example: new ViewDataDictionary&lt;User&gt;()</param>
        /// <returns>the raw html content of the email view and its master page</returns>
        protected virtual string EmailBody(string viewName, string masterName=null, ViewDataDictionary viewData=null)
        {
            var result = new StringResult
            {
                ViewName = viewName,
                ViewData = viewData,
                MasterName = masterName ?? MasterName
            };
            ControllerContext = ControllerContext ?? CreateControllerContext();
            result.ExecuteResult(ControllerContext);
            return result.Output;
        }

        protected virtual string PopulateBody(MailMessage mailMessage, string viewName, string masterName = null, ViewDataDictionary viewData = null)
        {
            if(mailMessage == null)
            {
                mailMessage = new MailMessage();
            }
            mailMessage.IsBodyHtml = IsBodyHtml;
            mailMessage.Body = EmailBody(viewName, masterName, viewData);
            return mailMessage.Body;
        }

        private ControllerContext CreateControllerContext()
        {
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            return new ControllerContext(new HttpContextWrapper(HttpContext.Current), routeData, this);
        }
    }

}